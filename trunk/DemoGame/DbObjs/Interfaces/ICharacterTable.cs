/********************************************************************
                   DO NOT MANUALLY EDIT THIS FILE!

This file was automatically generated using the DbClassCreator
program. The only time you should ever alter this file is if you are
using an automated code formatter. The DbClassCreator will overwrite
this file every time it is run, so all manual changes will be lost.
If there is something in this file that you wish to change, you should
be able to do it through the DbClassCreator arguments.

Make sure that you re-run the DbClassCreator every time you alter your
game's database.

For more information on the DbClassCreator, please see:
    http://www.netgore.com/wiki/dbclasscreator.html
********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using NetGore.AI;
using NetGore.Features.NPCChat;
using NetGore.Features.Shops;
using NetGore.World;

namespace DemoGame.DbObjs
{
    /// <summary>
    /// Interface for a class that can be used to serialize values to the database table `character`.
    /// </summary>
    public interface ICharacterTable
    {
        /// <summary>
        /// Gets the value of the database column `ai_id`.
        /// </summary>
        AIID? AIID { get; }

        /// <summary>
        /// Gets the value of the database column `body_id`.
        /// </summary>
        BodyID BodyID { get; }

        /// <summary>
        /// Gets the value of the database column `cash`.
        /// </summary>
        Int32 Cash { get; }

        /// <summary>
        /// Gets the value of the database column `character_template_id`.
        /// </summary>
        CharacterTemplateID? CharacterTemplateID { get; }

        /// <summary>
        /// Gets the value of the database column `chat_dialog`.
        /// </summary>
        NPCChatDialogID? ChatDialog { get; }

        /// <summary>
        /// Gets the value of the database column `exp`.
        /// </summary>
        Int32 Exp { get; }

        /// <summary>
        /// Gets the value of the database column `hp`.
        /// </summary>
        SPValueType HP { get; }

        /// <summary>
        /// Gets the value of the database column `id`.
        /// </summary>
        CharacterID ID { get; }

        /// <summary>
        /// Gets the value of the database column `level`.
        /// </summary>
        Byte Level { get; }

        /// <summary>
        /// Gets the value of the database column `load_map_id`.
        /// </summary>
        MapID LoadMapID { get; }

        /// <summary>
        /// Gets the value of the database column `load_x`.
        /// </summary>
        UInt16 LoadX { get; }

        /// <summary>
        /// Gets the value of the database column `load_y`.
        /// </summary>
        UInt16 LoadY { get; }

        /// <summary>
        /// Gets the value of the database column `mp`.
        /// </summary>
        SPValueType MP { get; }

        /// <summary>
        /// Gets the value of the database column `move_speed`.
        /// </summary>
        UInt16 MoveSpeed { get; }

        /// <summary>
        /// Gets the value of the database column `name`.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Gets the value of the database column `respawn_map_id`.
        /// </summary>
        MapID? RespawnMapID { get; }

        /// <summary>
        /// Gets the value of the database column `respawn_x`.
        /// </summary>
        Single RespawnX { get; }

        /// <summary>
        /// Gets the value of the database column `respawn_y`.
        /// </summary>
        Single RespawnY { get; }

        /// <summary>
        /// Gets the value of the database column `shop_id`.
        /// </summary>
        ShopID? ShopID { get; }

        /// <summary>
        /// Gets the value of the database column `statpoints`.
        /// </summary>
        Int32 StatPoints { get; }

        /// <summary>
        /// Gets an IEnumerable of KeyValuePairs containing the values in the `Stat` collection. The
        /// key is the collection's key and the value is the value for that corresponding key.
        /// </summary>
        IEnumerable<KeyValuePair<StatType, Int32>> Stats { get; }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        ICharacterTable DeepCopy();

        /// <summary>
        /// Gets the value of the database column in the column collection `Stat`
        /// that corresponds to the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key that represents the column in this column collection.</param>
        /// <returns>
        /// The value of the database column with the corresponding <paramref name="key"/>.
        /// </returns>
        Int32 GetStat(StatType key);
    }
}