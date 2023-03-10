//using Godot;

//namespace GodotRollbackNetcode
//{
//    public class GDScriptWrapper : Godot.Reference
//    {
//        public Godot.Object Source { get; protected set; }
//        public GDScriptWrapper() { }
//        public GDScriptWrapper(Godot.Object source)
//        {
//            Source = source;
//            ForwardSignalsToEvents();
//        }

//        /// <summary>
//        /// Connects a signal to the source GDScript
//        /// </summary>
//        /// <param name="signal"></param>
//        /// <param name="target"></param>
//        /// <param name="method"></param>
//        /// <param name="binds"></param>
//        /// <param name="flags"></param>
//        /// <returns></returns>
//        public new Error Connect(string signal, Godot.Object target, string method, Godot.Collections.Array binds = null, uint flags = 0)
//        {
//            return Source.Connect(signal, target, method, binds, flags);
//        }

//        protected virtual void ForwardSignalsToEvents() { }
//    }
//}
