using System.Threading.Tasks;
using UnityEngine;

namespace ArBreakout.Gui.Modal
{
    public class SettingsModal : MonoBehaviour
    {
        private TaskCompletionSource<bool> _tsc;

        public Task Show()
        {
            _tsc = new TaskCompletionSource<bool>();

            return _tsc.Task;
        }
        
    }
}