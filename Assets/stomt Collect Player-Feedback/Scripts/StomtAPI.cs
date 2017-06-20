using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;

namespace Stomt
{
	/// <summary>
	/// A single stomt item.
	/// </summary>
	public struct StomtItem
	{
		public string Id { get; set; }
		public bool Positive { get; set; }
		public string Text { get; set; }
		public string Language { get; set; }
		public DateTime CreationDate { get; set; }
		public bool Anonym { get; set; }
		public string CreatorId { get; set; }
		public string CreatorName { get; set; }
	}


	/// <summary>
	/// Low-level stomt API component.
	/// </summary>
	public class StomtAPI : MonoBehaviour
	{
        public string restServerURL;
        public bool NetworkError { get; set; }

		/// <summary>
		/// References a method to be called when the asynchronous feed download completes.
		/// </summary>
		/// <param name="feed">The list of stomt items from the requested feed.</param>
		public delegate void FeedCallback(StomtItem[] feed);

		#region Inspector Variables
		[SerializeField]
		[Tooltip("The application ID for your game. Create one on https://www.stomt.com/dev/my-apps/.")]
		string _appId = "";
		[SerializeField]
		[Tooltip("The ID of the target page for your game on https://www.stomt.com/.")]
		string _targetId = "";
		#endregion
		string _accessToken = "";

		/// <summary>
		/// The application ID for your game.
		/// </summary>
		public string AppId
		{
			get { return _appId; }
		}
		/// <summary>
		/// The target page ID for your game.
		/// </summary>
		public string TargetId
		{
			get { return _targetId; }
		}
		/// <summary>
		/// The name of your target page.
		/// </summary>
		public string TargetName { get; set; }
        public string TargetImageURL { get; set; }
		/// <summary>
		/// Requests the asynchronous feed download from your game's target.
		/// </summary>
		/// <param name="callback">The <see cref="FeedCallback"/> delegate.</param>
		/// <param name="offset">The offset from feed begin.</param>
		/// <param name="limit">The maximum amount of stomts to load.</param>
		public void LoadFeed(FeedCallback callback, int offset = 0, int limit = 15)
		{
			LoadFeed(_targetId, callback, offset, limit);
		}
		/// <summary>
		/// Requests the asynchronous feed download from the specified target.
		/// </summary>
		/// <param name="target">The target to download the feed from.</param>
		/// <param name="callback">The <see cref="FeedCallback"/> delegate.</param>
		/// <param name="offset">The offset from feed begin.</param>
		/// <param name="limit">The maximum amount of stomts to load.</param>
		public void LoadFeed(string target, FeedCallback callback, int offset = 0, int limit = 15)
		{
			StartCoroutine(LoadFeedAsync(target, callback, offset, limit));
		}
		/// <summary>
		/// Creates a new anonymous stomt on the game's target.
		/// </summary>
		/// <param name="positive">The stomt type. True for "I like" and false for "I wish".</param>
		/// <param name="text">The stomt message.</param>
		public void CreateStomt(bool positive, string text)
		{
			CreateStomt(positive, _targetId, text);
		}
		/// <summary>
		/// Creates a new anonymous stomt on the specified target.
		/// </summary>
		/// <param name="positive">The stomt type. True for "I like" and false for "I wish".</param>
		/// <param name="target">The target to post the stomt to.</param>
		/// <param name="text">The stomt message.</param>
		public void CreateStomt(bool positive, string target, string text)
		{
			var json = new StringBuilder();
			var writer = new LitJson.JsonWriter(json);

			writer.WriteObjectStart();
			writer.WritePropertyName("anonym");
			writer.Write(true);
			writer.WritePropertyName("positive");
			writer.Write(positive);
			writer.WritePropertyName("target_id");
			writer.Write(target);
			writer.WritePropertyName("text");
			writer.Write(text);
			writer.WriteObjectEnd();

			StartCoroutine(CreateStomtAsync(json.ToString()));
		}
		/// <summary>
		/// Creates a new anonymous stomt on the game's target with an image attached to it.
		/// </summary>
		/// <param name="positive">The stomt type. True for "I like" and false for "I wish".</param>
		/// <param name="text">The stomt message.</param>
		/// <param name="image">The image texture to upload and attach to the stomt.</param>
		public void CreateStomtWithImage(bool positive, string text, Texture2D image)
		{
			CreateStomtWithImage(positive, _targetId, text, image);
		}
		/// <summary>
		/// Creates a new anonymous stomt on the specified target with an image attached to it.
		/// </summary>
		/// <param name="positive">The stomt type. True for "I like" and false for "I wish".</param>
		/// <param name="target">The target to post the stomt to.</param>
		/// <param name="text">The stomt message.</param>
		/// <param name="image">The image texture to upload and attach to the stomt.</param>
		public void CreateStomtWithImage(bool positive, string target, string text, Texture2D image)
		{
			if (image == null)
			{
				CreateStomt(positive, target, text);
				return;
			}

			byte[] imageBytes = image.EncodeToPNG();

			if (imageBytes == null)
			{
				return;
			}

			var jsonImage = new StringBuilder();
			var writerImage = new LitJson.JsonWriter(jsonImage);

			writerImage.WriteObjectStart();
			writerImage.WritePropertyName("images");
			writerImage.WriteObjectStart();
			writerImage.WritePropertyName("stomt");
			writerImage.WriteArrayStart();
			writerImage.WriteObjectStart();
			writerImage.WritePropertyName("data");
			writerImage.Write(Convert.ToBase64String(imageBytes));
			writerImage.WriteObjectEnd();
			writerImage.WriteArrayEnd();
			writerImage.WriteObjectEnd();
			writerImage.WriteObjectEnd();

			var jsonStomt = new StringBuilder();
			var writerStomt = new LitJson.JsonWriter(jsonStomt);

			writerStomt.WriteObjectStart();
			writerStomt.WritePropertyName("anonym");
			writerStomt.Write(true);
			writerStomt.WritePropertyName("positive");
			writerStomt.Write(positive);
			writerStomt.WritePropertyName("target_id");
			writerStomt.Write(target);
			writerStomt.WritePropertyName("text");
			writerStomt.Write(text);
			writerStomt.WritePropertyName("img_name");
			writerStomt.Write("{img_name}");
			writerStomt.WriteObjectEnd();

			StartCoroutine(CreateStomtWithImageAsync(jsonImage.ToString(), jsonStomt.ToString()));
		}

        void Awake()
        {
            StartCoroutine(LoadTarget(_targetId));
            NetworkError = false;
        }

		void Start()
		{
			if (string.IsNullOrEmpty(_appId))
			{
				throw new ArgumentException("The stomt application ID variable cannot be empty.");
			}
			if (string.IsNullOrEmpty(_targetId))
			{
				throw new ArgumentException("The stomt target ID variable cannot be empty.");
			}

			// TODO: Workaround to accept the stomt SSL certificate. This should be replaced with a proper solution.
			ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

			// Load target information
			TargetName = _targetId;
			StartCoroutine(LoadTarget(_targetId));
		}

		HttpWebRequest WebRequest(string method, string url)
		{
			var request = (HttpWebRequest)System.Net.WebRequest.Create(url);
			request.Method = method;
			request.Accept = request.ContentType = "application/json";
			request.UserAgent = string.Format("Unity/{0} ({1})", Application.unityVersion, Application.platform);
			request.Headers["appid"] = _appId;

			if (!string.IsNullOrEmpty(_accessToken))
			{
				request.Headers["accesstoken"] = _accessToken;
			}

			return request;
		}
		
		IEnumerator LoadTarget(string target)
		{
			HttpWebRequest request = WebRequest("GET", string.Format("{0}/targets/{1}", restServerURL, target));

			// Send request and wait for response
			var async1 = request.BeginGetResponse(null, null);

			while (!async1.IsCompleted)
			{
				yield return null;
			}

			HttpWebResponse response;
			var responseDataText = string.Empty;

			try
			{
				response = (HttpWebResponse)request.EndGetResponse(async1);
                this.NetworkError = false;
			}
			catch (WebException ex)
			{
                this.NetworkError = true;
                Debug.LogException(ex);
                Debug.Log("Maybe wrong target id");
				yield break;
			}

			// Store access token
			_accessToken = response.Headers["accesstoken"];

			// Read response stream
			using (var responseStream = response.GetResponseStream())
			{
				if (responseStream == null)
				{
					yield break;
				}

				var buffer = new byte[2048];
				int length;

				while ((length = responseStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					responseDataText += Encoding.UTF8.GetString(buffer, 0, length);
				}
			}

			// Analyze JSON data
			LitJson.JsonData responseData = LitJson.JsonMapper.ToObject(responseDataText);

			if (responseData.Keys.Contains("error"))
			{
				Debug.LogError((string)responseData["error"]["msg"]);
				yield break;
			}

			responseData = responseData["data"];

			TargetName = (string)responseData["displayname"];
            TargetImageURL = (string)responseData["images"]["profile"][0];
		}
		
		IEnumerator LoadFeedAsync(string target, FeedCallback callback, int offset, int limit)
		{
			HttpWebRequest request = WebRequest("GET", string.Format("{0}/targets/{1}/stomts/received?offset={2}&limit={3}", restServerURL, target, offset, limit));

			// Send request and wait for response
			var async1 = request.BeginGetResponse(null, null);

			while (!async1.IsCompleted)
			{
				yield return null;
			}

			HttpWebResponse response;
			var responseDataText = string.Empty;

			try
			{
				response = (HttpWebResponse)request.EndGetResponse(async1);
			}
			catch (WebException ex)
			{
				Debug.LogException(ex);
				yield break;
			}

			// Store access token
			_accessToken = response.Headers["accesstoken"];

			// Read response stream
			using (var responseStream = response.GetResponseStream())
			{
				if (responseStream == null)
				{
					yield break;
				}

				var buffer = new byte[2048];
				int length;

				while ((length = responseStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					responseDataText += Encoding.UTF8.GetString(buffer, 0, length);
				}
			}

			// Analyze JSON data
			LitJson.JsonData responseData = LitJson.JsonMapper.ToObject(responseDataText);

			if (responseData.Keys.Contains("error"))
			{
				Debug.LogError((string)responseData["error"]["msg"]);
				yield break;
			}

			responseData = responseData["data"];

			var feed = new StomtItem[responseData.Count];

			for (int i = 0; i < responseData.Count; i++)
			{
				var item = responseData[i];

				feed[i] = new StomtItem {
					Id = (string)item["id"],
					Positive = (bool)item["positive"],
					Text = (string)item["text"],
					Language = (string)item["lang"],
					CreationDate = DateTime.Parse((string)item["created_at"]),
					Anonym = (bool)item["anonym"]
				};

				if (feed[i].Anonym)
				{
					continue;
				}

				feed[i].CreatorId = (string)item["creator"]["id"];
				feed[i].CreatorName = (string)item["creator"]["displayname"];
			}

			callback(feed);
		}
		
		IEnumerator CreateStomtAsync(string json)
		{
			var data = Encoding.UTF8.GetBytes(json);

            HttpWebRequest request = WebRequest("POST", string.Format("{0}/stomts", restServerURL));
			request.ContentLength = data.Length;

			// Send request
			var async1 = request.BeginGetRequestStream(null, null);

			while (!async1.IsCompleted)
			{
				yield return null;
			}

			try
			{
				using (var requestStream = request.EndGetRequestStream(async1))
				{
					requestStream.Write(data, 0, data.Length);
				}
			}
			catch (WebException ex)
			{
				Debug.LogException(ex);
				yield break;
			}

			// Wait for response
			var async2 = request.BeginGetResponse(null, null);

			while (!async2.IsCompleted)
			{
				yield return null;
			}

			HttpWebResponse response;

			try
			{
				response = (HttpWebResponse)request.EndGetResponse(async2);
			}
			catch (WebException ex)
			{
				Debug.LogException(ex);
				yield break;
			}

			// Store access token
			_accessToken = response.Headers["accesstoken"];
		}
		
		IEnumerator CreateStomtWithImageAsync(string jsonImage, string jsonStomt)
		{
			var data = Encoding.UTF8.GetBytes(jsonImage);

            HttpWebRequest request = WebRequest("POST", string.Format("{0}/images", restServerURL));
			request.ContentLength = data.Length;

			// Send request
			var async1 = request.BeginGetRequestStream(null, null);

			while (!async1.IsCompleted)
			{
				yield return null;
			}

			try
			{
				using (var requestStream = request.EndGetRequestStream(async1))
				{
					requestStream.Write(data, 0, data.Length);
				}
			}
			catch (WebException ex)
			{
				Debug.LogException(ex);
				yield break;
			}

			// Wait for response
			var async2 = request.BeginGetResponse(null, null);

			while (!async2.IsCompleted)
			{
				yield return null;
			}

			HttpWebResponse response;
			var responseDataText = string.Empty;

			try
			{
				response = (HttpWebResponse)request.EndGetResponse(async2);
                this.NetworkError = false;
			}
			catch (WebException ex)
			{ 
                this.NetworkError = true;
				Debug.LogException(ex);
				yield break;
                
			}

			// Store access token
			_accessToken = response.Headers["accesstoken"];

			// Read response stream
			using (var responseStream = response.GetResponseStream())
			{
				if (responseStream == null)
				{
					yield break;
				}

				var buffer = new byte[2048];
				int length;

				while ((length = responseStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					responseDataText += Encoding.UTF8.GetString(buffer, 0, length);
				}
			}

			// Analyze JSON data
			LitJson.JsonData responseData = LitJson.JsonMapper.ToObject(responseDataText);

			if (responseData.Keys.Contains("error"))
			{
				Debug.LogError((string)responseData["error"]["msg"]);
				yield break;
			}

			var imagename = (string)responseData["data"]["images"]["stomt"]["name"];

			yield return StartCoroutine(CreateStomtAsync(jsonStomt.Replace("{img_name}", imagename)));
		}


        public WWW LoadTargetImage()
        {
            
            // Start download
            if(TargetImageURL != null)
            { 
                var www = new WWW(TargetImageURL);
                while (!www.isDone)
                {
                    // wait until the download is done
                }

                return www;
            }
            else
            {
                return null;
            }
        }
	}
}
