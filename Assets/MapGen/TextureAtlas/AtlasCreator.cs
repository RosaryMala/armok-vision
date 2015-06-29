//#define DEBUG_ATLASES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AtlasCreator
{
    public static int AtlasSize = 2046;

    public class AtlasNode
    {
        public AtlasNode[] child = null;
        public Rect rc = new Rect(0, 0, 0, 0);
        public Texture2D imageRef = null;
        public bool hasImage = false;
        public int sortIndex = 0;
        public string name = "Unknown";

        private static readonly int TEXTURE_PADDING = 16;
        private static readonly bool BLEED = true;

        // The insert function traverses the tree looking for a place to insert the texture. 
        // It returns the node of the atlas the texture can go into or null to say it can't fit. 
        // Note we really don't have to store the rectangle for each node.
        // All we need is a split direction and coordinate like in a kd-tree, but it's more convenient with rects.
        public AtlasNode Insert(Texture2D image, int index)
        {
            if (image == null) // Obviously an error!
                return null;

            if (child != null)
            {// If this node is not a leaf, try inserting into first child.
                AtlasNode newNode = child[0].Insert(image, index);
                if (newNode != null)
                    return newNode;

                // No more room in first child, insert into second child!
                return child[1].Insert(image, index);
            }
            else
            {
                // If there is already a lightmap in this node, early out
                if (hasImage)
                    return null;

                // If this node is too small for the image, return
                if (!ImageFits(image, rc))
                    return null;

                // If the image is perfect, accept!
                if (PerfectFit(image, rc))
                {
                    hasImage = true;
                    imageRef = image;
                    name = imageRef.name;
                    sortIndex = index;
                    return this;
                }

                // If we made it this far, this node must be split.
                child = new AtlasNode[2];
                child[0] = new AtlasNode();
                child[1] = new AtlasNode();

                // Decide which way to split image
                float deltaW = rc.width - image.width;
                float deltaH = rc.height - image.height;

                if (deltaW > deltaH)
                {
                    child[0].rc = new Rect(rc.xMin, rc.yMin, image.width, rc.height);
                    child[1].rc = new Rect(rc.xMin + image.width + TEXTURE_PADDING, rc.yMin, rc.width - (image.width + TEXTURE_PADDING), rc.height);
                }
                else
                {
                    child[0].rc = new Rect(rc.xMin, rc.yMin, rc.width, image.height);
                    child[1].rc = new Rect(rc.xMin, rc.yMin + image.height + TEXTURE_PADDING, rc.width, rc.height - (image.height + TEXTURE_PADDING));
                }

                // Lets try inserting into first child, eh?
                return child[0].Insert(image, index);
            }
        }

        public bool Contains(string search)
        {
            if (name == search)
                return true;

            if (child != null)
            {
                if (child[0].Contains(search))
                    return true;
                if (child[1].Contains(search))
                    return true;
            }

            return false;
        }

        static bool ImageFits(Texture2D image, Rect rect)
        {
            return rect.width >= image.width && rect.height >= image.height;
        }

        static bool PerfectFit(Texture2D image, Rect rect)
        {
            return rect.width == image.width && rect.height == image.height;
        }

        public void GetNames(ref List<string> result)
        {
            if (hasImage)
            {
                result.Add(name);
            }
            if (child != null)
            {
                if (child[0] != null)
                {
                    child[0].GetNames(ref result);
                }
                if (child[1] != null)
                {
                    child[1].GetNames(ref result);
                }
            }
        }

        public void GetBounds(ref List<AtlasNode> result)
        {
            if (hasImage)
            {
                result.Add(this);
            }
            if (child != null)
            {
                if (child[0] != null)
                {
                    child[0].GetBounds(ref result);
                }
                if (child[1] != null)
                {
                    child[1].GetBounds(ref result);
                }
            }
        }

        public void Clear()
        {
            if (child != null)
            {
                if (child[0] != null)
                {
                    child[0].Clear();
                }
                if (child[1] != null)
                {
                    child[1].Clear();
                }
            }
            if (imageRef != null)
                imageRef = null;
        }

        int clampedCoord(int x, int y)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            if (x >= imageRef.width)
                x = imageRef.width - 1;
            if (y >= imageRef.height)
                y = imageRef.height - 1;
            return x + y * imageRef.width;
        }

        public void Build(Texture2D target)
        {
            if (child != null)
            {
                if (child[0] != null)
                {
                    child[0].Build(target);
                }
                if (child[1] != null)
                {
                    child[1].Build(target);
                }
            }
            if (imageRef != null)
            {
                Color[] data = imageRef.GetPixels(0);
                if (!BLEED)
                {
                    for (int x = 0; x < imageRef.width; ++x)
                    {
                        for (int y = 0; y < imageRef.height; ++y)
                        {
                            target.SetPixel(x + (int)rc.x, y + (int)rc.y, data[clampedCoord(x, y)]);
                        }
                    }
                }
                else
                    {
                        for (int x = 0; x < imageRef.width + TEXTURE_PADDING; ++x)
                        {
                            for (int y = 0; y < imageRef.height + TEXTURE_PADDING; ++y)
                            {
                                target.SetPixel(x + (int)rc.x - (TEXTURE_PADDING / 2), y + (int)rc.y - (TEXTURE_PADDING / 2), data[clampedCoord(x - (TEXTURE_PADDING / 2), y - (TEXTURE_PADDING / 2))]);
                            }
                        }
                    }
            }
        }
    }

    public class AtlasDescriptor
    {
        public string name;
        public Rect uvRect;
        public Matrix4x4 UVTransform
        {
            get
            {
                return Matrix4x4.TRS(uvRect.position, Quaternion.identity, uvRect.size);
            }
        }
        public int width;
        public int height;
    }

    public class Atlas
    {
        public Texture2D texture;
        public AtlasNode root;
        public AtlasDescriptor[] uvRects;
    }

    public static void SaveAtlas(Atlas atlas, string name)
    {
        if (atlas == null || atlas.texture == null)
            return;

        var bytes = atlas.texture.EncodeToPNG();

        if (!System.IO.Directory.Exists("./Debug/"))
            System.IO.Directory.CreateDirectory("./Debug/");

        //string name = Time.realtimeSinceStartup.ToString().Replace(".", "-"); //DateTime.UtcNow.ToString().Replace("/", "_").Replace(" ", "_").Replace("\\", "_");
        string file = "./Debug/" + name + ".png";

        System.IO.File.WriteAllBytes(file, bytes);
        Debug.Log("SAVE TO: " + file);
    }

    public static Atlas[] CreateAtlas(string name, Texture2D[] textures, Atlas startWith = null)
    {
        List<Texture2D> toProcess = new List<Texture2D>();
        toProcess.AddRange(textures);
        int index = toProcess.Count - 1;
        toProcess.Reverse(); // Because we index backwards

        List<Atlas> result = new List<Atlas>();

        int insertIndex = 0;
        if (startWith != null)
        {
            insertIndex = startWith.root.sortIndex;
        }

        while (index >= 0)
        {
            Atlas _atlas = startWith;
            if (_atlas == null)
            {
                _atlas = new Atlas();
                _atlas.texture = new Texture2D(AtlasSize, AtlasSize, TextureFormat.RGBA32, true);
                _atlas.texture.filterMode = FilterMode.Bilinear;
                _atlas.root = new AtlasNode();
                _atlas.root.rc = new Rect(0, 0, AtlasSize, AtlasSize);
            }
            startWith = null;

            while (index >= 0 && (_atlas.root.Contains(toProcess[index].name) || _atlas.root.Insert(toProcess[index], insertIndex++) != null))
            {
                index -= 1;
            }
            result.Add(_atlas);
            _atlas.root.sortIndex = insertIndex;
            insertIndex = 0;
            _atlas = null;
        }

        foreach (Atlas atlas in result)
        {
            atlas.root.Build(atlas.texture);
            List<AtlasNode> nodes = new List<AtlasNode>();
            atlas.root.GetBounds(ref nodes);
            nodes.Sort(delegate(AtlasNode x, AtlasNode y)
            {
                if (x.sortIndex == y.sortIndex) return 0;
                if (y.sortIndex > x.sortIndex) return -1;
                return 1;
            });

            List<Rect> rects = new List<Rect>();
            foreach (AtlasNode node in nodes)
            {
                Rect normalized = new Rect(node.rc.xMin / atlas.root.rc.width, node.rc.yMin / atlas.root.rc.height, node.rc.width / atlas.root.rc.width, node.rc.height / atlas.root.rc.height);
                // bunp everything over by half a pixel to avoid floating errors
                normalized.x += 0.5f / atlas.root.rc.width;
                normalized.width -= 1.0f / atlas.root.rc.width;
                normalized.y += 0.5f / atlas.root.rc.height;
                normalized.height -= 1.0f / atlas.root.rc.height;
                rects.Add(normalized);
            }

            atlas.uvRects = new AtlasDescriptor[rects.Count];
            for (int i = 0; i < rects.Count; i++)
            {
                atlas.uvRects[i] = new AtlasDescriptor();
                atlas.uvRects[i].width = (int)nodes[i].rc.width;
                atlas.uvRects[i].height = (int)nodes[i].rc.height;
                atlas.uvRects[i].name = nodes[i].name;
                atlas.uvRects[i].uvRect = rects[i];
            }

            atlas.root.Clear();
#if DEBUG_ATLASES
			atlas.texture.Apply(false, false);
			SaveAtlas(atlas, name);		
#else
            if (atlas != result[result.Count - 1])
                atlas.texture.Apply(true, true);
            else
                atlas.texture.Apply(true, false);
#endif
        }

        return result.ToArray();
    }
}
