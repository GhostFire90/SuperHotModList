using BepInEx;
using BepInEx.Bootstrap;
using SuperhotMenuMod;
using System.Collections.Generic;
using System.Linq;

namespace SuperhotModList
{
    [BepInPlugin("SHModList", "Super Hot Mod List", "1.0")]
    [BepInDependency("superhotMenuModifier")]
    public class SHModList : BaseUnityPlugin
    {
        class empty_entry : SHGUIview
        {
            public override void OnEnter()
            {
                SHGUI.current.PopView();
                base.OnEnter();
            }
        }
        class toggle_entry : SHGUIview
        {
            static uint entry_count = 0;
            static KeyValuePair<string, PluginInfo>[] plugin_info = null;
            KeyValuePair<string, PluginInfo> entry;
            toggle_entry()
            {
                if (plugin_info == null) plugin_info = Chainloader.PluginInfos.ToArray();
                entry = plugin_info[entry_count];
                entry_count++;
            }
            public override void OnEnter()
            {
                IToggleableMod toggleable = (IToggleableMod)entry.Value.Instance;
                toggleable.SetEnabled(!toggleable.GetEnabled());
                SHGUI.current.PopView();
                base.OnEnter();
            }

        }
        void Awake()
        {
            MenuEntry Mods_dir = new MenuEntry("mods", MenuEntry.Entry_Type.Directory, null);
            var plugins = Chainloader.PluginInfos;
            foreach(var plugin in plugins)
            {
                if(plugin.Value.Dependencies.Contains(new BepInDependency("SHModList", BepInDependency.DependencyFlags.SoftDependency)) && plugin.Value.Instance is IToggleableMod)
                {
                    
                    Mods_dir.AddChild(new MenuEntry(plugin.Value.Metadata.Name, MenuEntry.Entry_Type.App, typeof(toggle_entry)));

                }
                else
                {
                    Mods_dir.AddChild(new MenuEntry(plugin.Value.Metadata.Name, MenuEntry.Entry_Type.App, typeof(empty_entry)));

                }
            }
            SuperhotMenu.RegisterMenuEntry(Mods_dir);
        }
        /// <summary>
        /// If you want your mod to be toggleable in this list, your BaseUnityPlugin must implement this interface
        /// recommended to basically just put in your update an if statement for a bool somewhere that Set enabled writes to
        /// </summary>
        
    }
    public interface IToggleableMod
    {
        void SetEnabled(bool enabled);
        bool GetEnabled();
    }
}
