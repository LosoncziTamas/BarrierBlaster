using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ArBreakout.Misc.Debug
{
    public class InputReader : MonoBehaviour
    {
        private BinaryFormatter _binaryFormatter;
        private FileStream _readStream;
        private InputRecorder.InputState _readInput;

        public InputRecorder.InputState ReadInput() => _readInput;

        private bool _reading;
        private void Awake()
        {
            _binaryFormatter = new BinaryFormatter();
        }

        public void StartReading(string levelName)
        {
            _reading = true;
            var fileName = $"{Application.persistentDataPath}/input_{levelName}.dat";
            UnityEngine.Debug.Log($"Start reading: {fileName}");
            if (File.Exists(fileName))
            {
                _reading = true;
                _readStream = File.Open(fileName, FileMode.Open);
            }
            else
            {
                UnityEngine.Debug.LogError($"{fileName} doesn't exist.");
            }
        }

        public void EndReading()
        {
            _reading = false;
            _readStream?.Close();
            _readStream = null;
            UnityEngine.Debug.Log($"End reading.");
        }
        
        private void FixedUpdate()
        {            
            if (_reading)
            {
                if (_readStream.Position < _readStream.Length)
                {
                    _readInput = (InputRecorder.InputState)_binaryFormatter.Deserialize(_readStream);
                    UnityEngine.Debug.Log($"left: {_readInput.leftButton} right: {_readInput.rightButton} fire: {_readInput.fireButton}");
                }
                else
                {
                    EndReading();
                }
            }
        }
    }
}