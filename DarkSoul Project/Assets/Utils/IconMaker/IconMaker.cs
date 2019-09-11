using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SA.Utilities
{
	[ExecuteInEditMode]
	public class IconMaker : MonoBehaviour
	{


		public int height = 128;
		public int width = 128;

		public bool create;
		public RenderTexture ren;
		public Camera bakeCam;

		public string spriteName;
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			if (create)
			{
				CreateIcon();
				create = false;
			}
		}

		private void CreateIcon()
		{
			if (string.IsNullOrEmpty(spriteName))
			{ 
				spriteName = "icon";
			}

			
			
			string path = SaveLocation();
			path += spriteName;

//			ren.width = width; 
//			ren.height = height;
			
			RenderTexture currentRT = RenderTexture.active;
			bakeCam.targetTexture.Release();
			RenderTexture.active = bakeCam.targetTexture;
			bakeCam.Render();
			
			Texture2D imgPng =
				new Texture2D(bakeCam.targetTexture.width,
					bakeCam.targetTexture.height,
					TextureFormat.ARGB32,false);
			imgPng.ReadPixels(new Rect(0,0,bakeCam.targetTexture.width,bakeCam.targetTexture.height),0,0);
			RenderTexture.active = currentRT;
			byte[] bytesPng = imgPng.EncodeToPNG();
			File.WriteAllBytes(path+".png",bytesPng);
			
			Debug.Log(spriteName + " created");

		}

		string SaveLocation()
		{
			string saveLocation = Application.streamingAssetsPath + "/Icons/";
			if (!Directory.Exists(saveLocation))
			{
				Directory.CreateDirectory(saveLocation);
			}

			return saveLocation;
		}
	}
	

}
