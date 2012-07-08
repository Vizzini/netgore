﻿using System;
using System.Collections.Generic;
using System.Linq;
using NetGore.Graphics;
using NetGore.Graphics.GUI;
using SFML.Graphics;

namespace NetGore.Features.Quests
{
    public class QuestDescriptionListBox : ListBox<IQuestDescription>
    {
        static readonly Color _defaultCanTurnInColor = new Color(0, 150, 0);
        static readonly Color _defaultCannotStartColor = new Color(150, 0, 0);

        readonly Func<QuestID, bool> _hasFinishQuestReqs;
        readonly Func<QuestID, bool> _hasStartQuestReqs;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestDescriptionListBox"/> class.
        /// </summary>
        /// <param name="parent">Parent <see cref="Control"/> of this <see cref="Control"/>.</param>
        /// <param name="position">Position of the Control reletive to its parent.</param>
        /// <param name="clientSize">The size of the <see cref="Control"/>'s client area.</param>
        /// <param name="hasStartQuestReqs">A func used to check if the user has the requirements to start a quest.</param>
        /// <param name="hasFinishQuestReqs">A func used to check if the user has the requirements to finish a quest.</param>
        /// <exception cref="NullReferenceException"><paramref name="parent"/> is null.</exception>
        public QuestDescriptionListBox(Control parent, Vector2 position, Vector2 clientSize, Func<QuestID, bool> hasStartQuestReqs,
                                       Func<QuestID, bool> hasFinishQuestReqs) : base(parent, position, clientSize)
        {
            _hasStartQuestReqs = hasStartQuestReqs;
            _hasFinishQuestReqs = hasFinishQuestReqs;

            CanTurnInQuestForeColor = _defaultCanTurnInColor;
            CannotStartQuestForeColor = _defaultCannotStartColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestDescriptionListBox"/> class.
        /// </summary>
        /// <param name="guiManager">The GUI manager this <see cref="Control"/> will be managed by.</param>
        /// <param name="position">Position of the Control reletive to its parent.</param>
        /// <param name="clientSize">The size of the <see cref="Control"/>'s client area.</param>
        /// <param name="hasStartQuestReqs">A func used to check if the user has the requirements to start a quest.</param>
        /// <param name="hasFinishQuestReqs">A func used to check if the user has the requirements to finish a quest.</param>
        /// <exception cref="ArgumentNullException"><paramref name="guiManager"/> is null.</exception>
        public QuestDescriptionListBox(IGUIManager guiManager, Vector2 position, Vector2 clientSize,
                                       Func<QuestID, bool> hasStartQuestReqs, Func<QuestID, bool> hasFinishQuestReqs)
            : base(guiManager, position, clientSize)
        {
            _hasStartQuestReqs = hasStartQuestReqs;
            _hasFinishQuestReqs = hasFinishQuestReqs;

            CanTurnInQuestForeColor = _defaultCanTurnInColor;
            CannotStartQuestForeColor = _defaultCannotStartColor;
        }

        /// <summary>
        /// Gets or sets the font color for a quest that can be turned in.
        /// </summary>
        public Color CanTurnInQuestForeColor { get; set; }

        /// <summary>
        /// Gets or sets the font color for a quest that the is available but the user does not have the
        /// requirements to start.
        /// </summary>
        public Color CannotStartQuestForeColor { get; set; }

        /// <summary>
        /// Gets or sets the items to display.
        /// </summary>
        public override IEnumerable<IQuestDescription> Items
        {
            get
            {
                var ret = base.Items;

                // Have the lowest-priority sort be on the quest ID
                ret = ret.OrderBy(x => x.QuestID);

                // Put quests that can be started before quests that are available but cannot be started
                ret = ret.OrderByDescending(x => _hasStartQuestReqs(x.QuestID));

                // Put quests that can be turned in above all other quests
                ret = ret.OrderByDescending(x => _hasFinishQuestReqs(x.QuestID));

                return ret;
            }
            set { base.Items = value; }
        }

        /// <summary>
        /// Gets the default item drawer.
        /// </summary>
        /// <returns>The default item drawer.</returns>
        protected override Action<ISpriteBatch, Vector2, int> GetDefaultItemDrawer()
        {
            return QuestDescriptionDrawer;
        }

        /// <summary>
        /// Gets if the user has the requirements to finish the given quest.
        /// </summary>
        /// <param name="questID">The ID of the quest to finish.</param>
        /// <returns>True if the user has the requirements to finish the quest with the given <paramref name="questID"/>;
        /// otherwise false.</returns>
        protected virtual bool HasFinishQuestReqs(QuestID questID)
        {
            return _hasFinishQuestReqs(questID);
        }

        /// <summary>
        /// Gets if the user has the requirements to start the given quest.
        /// </summary>
        /// <param name="questID">The ID of the quest to start.</param>
        /// <returns>True if the user has the requirements to start the quest with the given <paramref name="questID"/>;
        /// otherwise false.</returns>
        protected virtual bool HasStartQuestReqs(QuestID questID)
        {
            return _hasStartQuestReqs(questID);
        }

        void QuestDescriptionDrawer(ISpriteBatch sb, Vector2 pos, int index)
        {
            var item = Items.ElementAtOrDefault(index);
            if (item == null)
                return;

            // Write the list index
            var indexStr = "  " + (index + 1) + ". ";
            var indexStrWidth = Font.MeasureString(indexStr).X;
            sb.DrawString(Font, indexStr, pos, ForeColor);

            // Get the color to use for the title
            var titleColor = ForeColor;
            if (HasFinishQuestReqs(item.QuestID))
                titleColor = CanTurnInQuestForeColor;
            else if (!HasStartQuestReqs(item.QuestID))
                titleColor = CannotStartQuestForeColor;

            // Draw the quest's title, prefixing a DONE tag if its ready to turn in
            var title = item.Name;
            if (_hasFinishQuestReqs(item.QuestID))
                title = "[DONE] " + title;
            sb.DrawString(Font, title, pos + new Vector2(indexStrWidth, 0), titleColor);
        }
    }
}