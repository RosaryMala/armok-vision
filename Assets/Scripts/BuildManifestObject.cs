using System.Collections.Generic;

#if !UNITY_CLOUD_BUILD
namespace UnityEngine.CloudBuild
{
    /// <summary>
    /// Build manifest object.
    /// <para>Pre export methods must take a single parameter of this type in order for the
    /// method to be called prior to building a project in Unity Cloud Build.
    /// </summary>
	public class BuildManifestObject : ScriptableObject
    {
        /// <summary>
        /// Tries the get a manifest value.
        /// </summary>
        /// <returns><c>true</c>, if <c>key</c> was found and can be cast to type <c>T</c>, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public bool TryGetValue<T>(string key, out T result)
        {
            result = default(T);
            return false;
        }

        /// <summary>
        /// Gets a manifest value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="key">Key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T GetValue<T>(string key)
        {
            return default(T);
        }

        /// <summary>
        /// Sets the value of the key specified.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void SetValue(string key, object value) { }

        /// <summary>
        /// Copy values from the specified <see cref="T:System.Collections.Generic.Dictionary"/>.
        /// </summary>
        /// <param name="source">Source <see cref="T:System.Collections.Generic.Dictionary"/>.</param>
        public void SetValues(Dictionary<string, object> source) { }

        /// <summary>
        /// Remove all key-value pairs.
        /// </summary>
        public void ClearValues() { }

        /// <summary>
        /// Returns a <see cref="T:System.Collections.Generic.Dictionary"/> that represents 
        /// the current <see cref="T:UnityEngine.CloudBuild.BuildManifestObject"/> instance.
        /// </summary>
        /// <returns>The <see cref="T:System.Collections.Generic.Dictionary"/>.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// Returns a JSON formatted <see cref="T:System.String"/> that represents 
        /// the current <see cref="T:UnityEngine.CloudBuild.BuildManifestObject"/> instance.
        /// </summary>
        /// <returns>The JSON formatted <see cref="T:System.String"/>.</returns>
        public string ToJson()
        {
            return null;
        }

        /// <summary>
        /// Returns an INI formatted <see cref="T:System.String"/> that represents 
        /// the current <see cref="T:UnityEngine.CloudBuild.BuildManifestObject"/> instance.
        /// </summary>
        /// <returns>The <see cref="T:System.String"/>.</returns>
        public override string ToString()
        {
            return null;
        }
    }
}
#endif