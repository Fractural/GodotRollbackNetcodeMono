using Godot;
using System.Linq;

namespace Game
{
    public class IPLabel : Label
    {
        public override void _Ready()
        {
            Text = string.Join(",\n", IP.GetLocalAddresses().Cast<string>().Where(x => x.Contains(".")));
        }
    }
}