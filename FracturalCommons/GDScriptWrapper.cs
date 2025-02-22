using Godot;

namespace GodotRollbackNetcode
{
    public partial class GDScriptWrapper : RefCounted
    {
        public GodotObject Source { get; protected set; }
        public GDScriptWrapper() { }
        public GDScriptWrapper(GodotObject source)
        {
            Source = source;
            ForwardSignalsToEvents();
        }

        /// <summary>
        /// Connects a signal to the source GDScript
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="binds"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Error Connect(string signal, Callable callable, uint flags = 0)
        {
            return Source.Connect(signal, callable, flags);
        }

        protected virtual void ForwardSignalsToEvents() { }
    }
}
