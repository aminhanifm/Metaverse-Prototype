using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MetaversePrototype.Tools
{
	public static class MPSaveLoadManager
	{
		/// the method to use when saving and loading files (has to be the same at both times of course)
		public static IMPSaveLoadManagerMethod saveLoadMethod = new MPSaveLoadManagerMethodBinary();
		/// the default top level folder the system will use to save the file
		private const string _baseFolderName = "/MPData/";
		/// the name of the save folder if none is provided
		private const string _defaultFolderName = "MPSaveLoadManager";

		static string DetermineSavePath(string folderName = _defaultFolderName)
		{
			string savePath;
			// depending on the device we're on, we assemble the path
			if (Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				savePath = Application.persistentDataPath + _baseFolderName;
			} 
			else 
			{
				savePath = Application.persistentDataPath + _baseFolderName;
			}
			#if UNITY_EDITOR
			savePath = Application.dataPath + _baseFolderName;
			#endif

			savePath = savePath + folderName + "/";
			return savePath;
		}

		static string DetermineSaveFileName(string fileName)
		{
			return fileName;
		}

		public static void Save(object saveObject, string fileName, string foldername = _defaultFolderName)
		{
			string savePath = DetermineSavePath(foldername);
			string saveFileName = DetermineSaveFileName(fileName);
			// if the directory doesn't already exist, we create it
			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
			// we serialize and write our object into a file on disk

			FileStream saveFile = File.Create(savePath + saveFileName);

			saveLoadMethod.Save(saveObject, saveFile);
			saveFile.Close();
		}

		public static object Load(System.Type objectType, string fileName, string foldername = _defaultFolderName)
		{
			string savePath = DetermineSavePath(foldername);
			string saveFileName = savePath + DetermineSaveFileName(fileName);

			object returnObject;

			// if the MPSaves directory or the save file doesn't exist, there's nothing to load, we do nothing and exit
			if (!Directory.Exists(savePath) || !File.Exists(saveFileName))
			{
				return null;
			}

			FileStream saveFile = File.Open(saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			returnObject = saveLoadMethod.Load(objectType, saveFile);
			saveFile.Close();

			return returnObject;
		}

		public static void DeleteSave(string fileName, string folderName = _defaultFolderName)
		{
			string savePath = DetermineSavePath(folderName);
			string saveFileName = DetermineSaveFileName(fileName);
			if (File.Exists(savePath + saveFileName))
			{
				File.Delete(savePath + saveFileName);
			}			
		}

		public static void DeleteSaveFolder(string folderName = _defaultFolderName)
		{
			string savePath = DetermineSavePath(folderName);
			if (Directory.Exists(savePath))
			{
				DeleteDirectory(savePath);
			}
		}

		public static void DeleteDirectory(string target_dir)
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(target_dir, false);
		}
	}
}