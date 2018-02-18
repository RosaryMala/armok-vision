using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Collada141
{
    public partial class COLLADA
    {
        public static geometry MeshToGeometry(Mesh inputMesh, string id = null)
        {
            string name;
            if (string.IsNullOrEmpty(id))
                name = "Mesh-" + inputMesh.GetInstanceID();
            else
                name = id;
            return MeshToGeometry(new CPUMesh(inputMesh), name);
        }
        public static geometry MeshToGeometry(CPUMesh inputMesh, string id)
        {

            geometry outputGeometry = new geometry();
            mesh outputMesh = new mesh();

            outputGeometry.id = id + "-lib";
            outputGeometry.name = inputMesh.name + "-mesh";
            outputGeometry.Item = outputMesh;


            //vertex Positions
            List<source> sourceList = new List<source>();
            var inputVertices = inputMesh.vertices;
            if (inputVertices.Length == 0)
                return null;
            sourceList.Add(ArrayToSource(inputMesh.vertices, id + "-POSITION"));

            vertices vertexList = new vertices();
            vertexList.id = id + "-VERTEX";
            vertexList.input = new InputLocal[1];
            vertexList.input[0] = new InputLocal();
            vertexList.input[0].semantic = "POSITION";
            vertexList.input[0].source = "#" + sourceList[0].id;
            outputMesh.vertices = vertexList;

            List<InputLocalOffset> offsetList = new List<InputLocalOffset>();

            {
                InputLocalOffset offset = new InputLocalOffset();
                offset.semantic = "VERTEX";
                offset.offset = 0;
                offset.source = "#" + vertexList.id;
                offsetList.Add(offset);
            }

            var inputNormals = inputMesh.normals;
            if(inputNormals.Length > 0)
            {
                var array = ArrayToSource(inputNormals, id + "-Normal0");
                InputLocalOffset offset = new InputLocalOffset();
                offset.semantic = "NORMAL";
                offset.offset = (ulong)sourceList.Count;
                offset.source = "#" + array.id;
                sourceList.Add(array);
                offsetList.Add(offset);
            }
            var inputUV1s = inputMesh.uv;
            if (inputUV1s.Length > 0)
            {
                var array = ArrayToSource(inputUV1s, id + "-UV0");
                InputLocalOffset offset = new InputLocalOffset();
                offset.semantic = "TEXCOORD";
                offset.offset = (ulong)sourceList.Count;
                offset.source = "#" + array.id;
                offset.set = 0;
                offset.setSpecified = true;
                sourceList.Add(array);
                offsetList.Add(offset);
            }
            var inputUV2s = inputMesh.uv2;
            if (inputUV2s.Length > 0)
            {
                var array = ArrayToSource(inputUV2s, id + "-UV1");
                InputLocalOffset offset = new InputLocalOffset();
                offset.semantic = "TEXCOORD";
                offset.offset = (ulong)sourceList.Count;
                offset.source = "#" + array.id;
                offset.set = 1;
                offset.setSpecified = true;
                sourceList.Add(array);
                offsetList.Add(offset);
            }
            var inputColors = inputMesh.colors;
            if (inputColors.Length > 0)
            {
                var array = ArrayToSource(inputColors, id + "-VERTEX_COLOR0");
                InputLocalOffset offset = new InputLocalOffset();
                offset.semantic = "COLOR";
                offset.offset = (ulong)sourceList.Count;
                offset.source = "#" + array.id;
                offset.set = 0;
                offset.setSpecified = true;
                sourceList.Add(array);
                offsetList.Add(offset);
            }

            outputMesh.source = sourceList.ToArray();


            triangles triangleList = new triangles();
            triangleList.input = offsetList.ToArray();

            var inputTriangles = inputMesh.triangles;

            triangleList.count = (ulong)inputTriangles.Length / 3;

            if (triangleList.count == 0)
                return null;

            StringBuilder pString = new StringBuilder();

            for(int i = 0; i < inputTriangles.Length; i++)
            {
                for(int j = 0; j < triangleList.input.Length; j++)
                {
                    pString.Append(inputTriangles[i]).Append(" ");
                }
                if (i % 3 == 2)
                    pString.AppendLine();
                else
                    pString.Append("   ");
            }

            triangleList.p = pString.ToString();

            outputMesh.Items = new object[1];
            outputMesh.Items[0] = triangleList;

            return outputGeometry;
        }

        public static source ArrayToSource(Vector3[] input, string tag)
        {
            source outputSource = new source();
            outputSource.id = tag;

            float_array vectorArray = new float_array();
            vectorArray.Values = new double[input.Length * 3];
            for (int i = 0; i < input.Length; i++)
            {
                vectorArray.Values[i * 3 + 0] = input[i].x;
                vectorArray.Values[i * 3 + 1] = input[i].y;
                vectorArray.Values[i * 3 + 2] = input[i].z;
            }
            vectorArray.id = tag + "-array";
            vectorArray.count = (ulong)vectorArray.Values.Length;

            outputSource.Item = vectorArray;

            accessor vectorAccessor = new accessor();
            vectorAccessor.source = "#" + tag + "-array";
            vectorAccessor.count = (ulong)input.Length;
            vectorAccessor.stride = 3;
            vectorAccessor.param = new param[3];
            vectorAccessor.param[0] = new param();
            vectorAccessor.param[0].name = "X";
            vectorAccessor.param[0].type = "float";
            vectorAccessor.param[1] = new param();
            vectorAccessor.param[1].name = "Y";
            vectorAccessor.param[1].type = "float";
            vectorAccessor.param[2] = new param();
            vectorAccessor.param[2].name = "Z";
            vectorAccessor.param[2].type = "float";

            outputSource.technique_common = new sourceTechnique_common();
            outputSource.technique_common.accessor = vectorAccessor;

            return outputSource;
        }

        public static source ArrayToSource(Color[] input, string tag)
        {
            source outputSource = new source();
            outputSource.id = tag;

            const int num_values = 4;

            float_array vectorArray = new float_array();
            vectorArray.Values = new double[input.Length * num_values];
            for (int i = 0; i < input.Length; i++)
            {
                vectorArray.Values[i * num_values + 0] = input[i].r;
                vectorArray.Values[i * num_values + 1] = input[i].g;
                vectorArray.Values[i * num_values + 2] = input[i].b;
                vectorArray.Values[i * num_values + 3] = input[i].a;
            }
            vectorArray.id = tag + "-array";
            vectorArray.count = (ulong)vectorArray.Values.Length;

            outputSource.Item = vectorArray;

            accessor vectorAccessor = new accessor();
            vectorAccessor.source = "#" + vectorArray.id;
            vectorAccessor.count = (ulong)input.Length;
            vectorAccessor.stride = num_values;
            vectorAccessor.param = new param[num_values];
            vectorAccessor.param[0] = new param();
            vectorAccessor.param[0].name = "R";
            vectorAccessor.param[0].type = "float";
            vectorAccessor.param[1] = new param();
            vectorAccessor.param[1].name = "G";
            vectorAccessor.param[1].type = "float";
            vectorAccessor.param[2] = new param();
            vectorAccessor.param[2].name = "B";
            vectorAccessor.param[2].type = "float";
            vectorAccessor.param[3] = new param();
            vectorAccessor.param[3].name = "A";
            vectorAccessor.param[3].type = "float";

            outputSource.technique_common = new sourceTechnique_common();
            outputSource.technique_common.accessor = vectorAccessor;

            return outputSource;
        }

        public static source ArrayToSource(Vector2[] input, string tag)
        {
            source outputSource = new source();
            outputSource.id = tag;

            const int num_values = 2;

            float_array vectorArray = new float_array();
            vectorArray.Values = new double[input.Length * num_values];
            for (int i = 0; i < input.Length; i++)
            {
                vectorArray.Values[i * num_values + 0] = input[i].x;
                vectorArray.Values[i * num_values + 1] = input[i].y;
            }
            vectorArray.id = tag + "-array";
            vectorArray.count = (ulong)vectorArray.Values.Length;

            outputSource.Item = vectorArray;

            accessor vectorAccessor = new accessor();
            vectorAccessor.source = "#" + vectorArray.id;
            vectorAccessor.count = (ulong)input.Length;
            vectorAccessor.stride = num_values;
            vectorAccessor.param = new param[num_values];
            vectorAccessor.param[0] = new param();
            vectorAccessor.param[0].name = "S";
            vectorAccessor.param[0].type = "float";
            vectorAccessor.param[1] = new param();
            vectorAccessor.param[1].name = "T";
            vectorAccessor.param[1].type = "float";

            outputSource.technique_common = new sourceTechnique_common();
            outputSource.technique_common.accessor = vectorAccessor;

            return outputSource;
        }

        public static matrix ConvertMatrix(Matrix4x4 input)
        {
            matrix output = new matrix();

            output.Values = new double[16];
            int i = 0;
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    output.Values[i] = input[x,y];
                    i++;
                }
            output.sid = "matrix";

            return output;
        }

        public static rotate ConvertRotation(Quaternion input)
        {
            rotate output = new rotate();

            Vector3 axis;
            float angle;
            input.ToAngleAxis(out angle, out axis);

            output.Values = new double[] { axis.x, axis.y, axis.z, angle };
            output.sid = "rotate";
            return output;
        }
        
        public static TargetableFloat3 ConvertVector(Vector3 input, string sid)
        {
            TargetableFloat3 output = new TargetableFloat3();
            output.Values = new double[] { input.x, input.y, input.z };
            return output;
        }
    }
}