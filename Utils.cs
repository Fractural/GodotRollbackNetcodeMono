using GDC = Godot.Collections;

namespace GodotRollbackNetcode
{
    public static class Utils
    {
        /// <summary>
        /// Fetchs value from the state dictionary regardless if the key starts with "_" or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stateDict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetStateValue<T>(this GDC.Dictionary stateDict, string key)
        {
            key = key.TrimStart('_');
            return stateDict.Get<T>(key);
        }

        /// <summary>
        /// Changes <paramref name="stateDict"/> keys to start with "_", which 
        /// makes the <see cref="IHashSerializer"/> ignore it when generating a hash 
        /// of the client's state.
        /// </summary>
        /// <param name="stateDict"></param>
        /// <returns></returns>
        public static GDC.Dictionary IgnoreState(this GDC.Dictionary stateDict)
        {
            var newDict = new GDC.Dictionary();
            foreach (var key in stateDict.Keys)
            {
                if (key.Obj is string strKey && !strKey.StartsWith("_"))
                    newDict["_" + key] = stateDict[key];
                else
                    newDict[key] = stateDict[key];
            }
            return newDict;
        }
    }
}
