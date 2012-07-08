﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DemoGame.Client;
using DemoGame.Editor.Properties;
using NetGore.Editor.EditorTool;
using ToolBar = NetGore.Editor.EditorTool.ToolBar;

namespace DemoGame.Editor.Tools
{
    /// <summary>
    /// A <see cref="Tool"/> that deletes the current map.
    /// </summary>
    public class MapPreviewTool : Tool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapPreviewTool"/> class.
        /// </summary>
        /// <param name="toolManager">The <see cref="ToolManager"/>.</param>
        protected MapPreviewTool(ToolManager toolManager) : base(toolManager, CreateSettings())
        {
            ToolBarControl.ControlSettings.ToolTipText = "Saves a preview image of the map to file";
            ToolBarControl.ControlSettings.Click += ControlSettings_Click;
        }

        void ControlSettings_Click(object sender, EventArgs e)
        {
            var tb = ToolBar.GetToolBar(ToolBarVisibility.Map);
            if (tb == null)
                return;

            var map = tb.DisplayObject as Map;
            if (map == null)
                return;

            var filePath = Path.GetFullPath(map.ID + ".png");

            var mp = new MapPreviewer();
            mp.CreatePreview(map, ToolManager.MapDrawingExtensions, filePath);

            MessageBox.Show("Saved map preview to file:" + Environment.NewLine + filePath, "Preview saved", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Creates the <see cref="ToolSettings"/> to use for instantiating this class.
        /// </summary>
        /// <returns>The <see cref="ToolSettings"/>.</returns>
        static ToolSettings CreateSettings()
        {
            return new ToolSettings("Map Preview")
            {
                ToolBarVisibility = ToolBarVisibility.Map,
                OnToolBarByDefault = true,
                ToolBarControlType = ToolBarControlType.Button,
                EnabledImage = Resources.MapPreviewTool,
            };
        }
    }
}