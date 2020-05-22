namespace Xap.Infrastructure.Events {
    public class XapEventArgs : System.EventArgs {
        /// <summary>
        /// Text of the message being returned to the UI
        /// </summary>
        public string msgData;
        public XapEventArgs(string msg) {
            msgData = msg;
        }
    }
}
