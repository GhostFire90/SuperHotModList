using BepInEx;
using BepInEx.Bootstrap;
using SuperhotMenuMod;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
            public static uint entry_count = 0;
            public static KeyValuePair<string, PluginInfo>[] plugin_info = null;
            public KeyValuePair<string, PluginInfo> entry;

            public toggle_entry()
            {
                entry = plugin_info[entry_count];
            }

            public override void OnEnter()
            {
                IToggleableMod toggleable = (IToggleableMod)entry.Value.Instance;
                bool mode = !toggleable.GetEnabled();
                toggleable.SetEnabled(mode);
                Debug.Log($"SHMod List toggled {entry.Value.Metadata.Name} to {mode}");
                SHGUI.current.PopView();
                base.OnEnter();
            }

        }
        

        void Start()
        {
            
            MenuEntry Mods_dir = new MenuEntry("mods", MenuEntry.Entry_Type.Directory, null);
            var plugins = Chainloader.PluginInfos;
            uint i = 0;
            foreach(var plugin in plugins)
            {
                if(plugin.Value.Dependencies.Contains(new BepInDependency("SHModList", BepInDependency.DependencyFlags.SoftDependency)) && plugin.Value.Instance is IToggleableMod)
                {
                    if (toggle_entry.plugin_info == null) toggle_entry.plugin_info = plugins.ToArray();
                    Mods_dir.AddChild(new MenuEntry(plugin.Value.Metadata.Name, MenuEntry.Entry_Type.App, typeof(toggle_entry)));
                    toggle_entry.entry_count = i;
                    
                }
                else
                {
                    Mods_dir.AddChild(new MenuEntry(plugin.Value.Metadata.Name, MenuEntry.Entry_Type.App, typeof(empty_entry)));

                }
                i++;
            }
            SuperhotMenu.RegisterMenuEntry(Mods_dir);
            SuperhotMenu.RePatch();
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
