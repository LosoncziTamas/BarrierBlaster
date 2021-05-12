using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ArBreakout.Misc.Debug
{
    public class InputRecorder : MonoBehaviour
    {
        [Serializable]
        public struct InputState
        {
            public bool leftButton;
            public bool rightButton;
            public bool fireButton;
        }
        
        private BinaryFormatter _binaryFormatter;
        private FileStream _saveStream;
        private bool _recording;

        private void Awake()
        {
            _binaryFormatter = new BinaryFormatter();
        }

        public void StartRecording(string levelName)
        {
            var fileName = $"{Application.persistentDataPath}/input_{levelName}.dat";
            UnityEngine.Debug.Log($"Start recording: {fileName}");
            _saveStream = File.Create(fileName);
            _recording = true;
        }

        public void EndRecording()
        {
            _recording = false;
            _saveStream?.Close();
            _saveStream = null;
            UnityEngine.Debug.Log($"End recording.");
        }

        private void FixedUpdate()
        {
            var input = new InputState
            {
                leftButton = Input.GetAxis("Horizontal") < 0,
                rightButton = Input.GetAxis("Horizontal") > 0,
                fireButton = Input.GetButton("Jump")
            };
            
            if (_recording)
            {
                _binaryFormatter.Serialize(_saveStream, input);   
            } 
        }
    }
}