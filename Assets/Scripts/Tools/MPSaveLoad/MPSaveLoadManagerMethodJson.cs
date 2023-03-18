using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace MetaversePrototype.Tools
{
	public class MPSaveLoadManagerMethodJson : IMPSaveLoadManagerMethod
	{
		public void Save(object objectToSave, FileStream saveFile)
		{
			string json = JsonUtility.ToJson(objectToSave);
			// if you prefer using NewtonSoft's JSON lib uncomment the line below and commment the line above
			//string json = Newtonsoft.Json.JsonConvert.SerializeObject(objectToSave);
			StreamWriter streamWriter = new StreamWriter(saveFile);
			streamWriter.Write(json);
			streamWriter.Close();
			saveFile.Close();
		}

		public object Load(System.Type objectType, FileStream saveFile)
		{
			object savedObject; // = System.Activator.CreateInstance(objectType);
			StreamReader streamReader = new StreamReader(saveFile, Encoding.UTF8);
			string json = streamReader.ReadToEnd();
			savedObject = JsonUtility.FromJson(json, objectType);
			// if you prefer using NewtonSoft's JSON lib uncomment the line below and commment the line above
			//savedObject = Newtonsoft.Json.JsonConvert.DeserializeObject(json,objectType);
			streamReader.Close();
			saveFile.Close();
			return savedObject;
		}
	}
}