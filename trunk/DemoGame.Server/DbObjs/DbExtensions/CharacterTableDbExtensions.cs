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
using System.Data;
using System.Linq;
using DemoGame.DbObjs;
using NetGore.AI;
using NetGore.Db;
using NetGore.Features.NPCChat;
using NetGore.Features.Shops;
using NetGore.World;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Contains extension methods for class CharacterTable that assist in performing
    /// reads and writes to and from a database.
    /// </summary>
    public static class CharacterTableDbExtensions
    {
        /// <summary>
        /// Copies the column values into the given DbParameterValues using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
        ///  this method will not create them if they are missing.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="paramValues">The DbParameterValues to copy the values into.</param>
        public static void CopyValues(this ICharacterTable source, DbParameterValues paramValues)
        {
            paramValues["ai_id"] = (ushort?)source.AIID;
            paramValues["body_id"] = (UInt16)source.BodyID;
            paramValues["cash"] = source.Cash;
            paramValues["character_template_id"] = (ushort?)source.CharacterTemplateID;
            paramValues["chat_dialog"] = (ushort?)source.ChatDialog;
            paramValues["exp"] = source.Exp;
            paramValues["hp"] = (Int16)source.HP;
            paramValues["id"] = (Int32)source.ID;
            paramValues["level"] = source.Level;
            paramValues["load_map_id"] = (UInt16)source.LoadMapID;
            paramValues["load_x"] = source.LoadX;
            paramValues["load_y"] = source.LoadY;
            paramValues["move_speed"] = source.MoveSpeed;
            paramValues["mp"] = (Int16)source.MP;
            paramValues["name"] = source.Name;
            paramValues["respawn_map_id"] = (ushort?)source.RespawnMapID;
            paramValues["respawn_x"] = source.RespawnX;
            paramValues["respawn_y"] = source.RespawnY;
            paramValues["shop_id"] = (ushort?)source.ShopID;
            paramValues["statpoints"] = source.StatPoints;
            paramValues["stat_agi"] = (Int16)source.GetStat(StatType.Agi);
            paramValues["stat_defence"] = (Int16)source.GetStat(StatType.Defence);
            paramValues["stat_int"] = (Int16)source.GetStat(StatType.Int);
            paramValues["stat_maxhit"] = (Int16)source.GetStat(StatType.MaxHit);
            paramValues["stat_maxhp"] = (Int16)source.GetStat(StatType.MaxHP);
            paramValues["stat_maxmp"] = (Int16)source.GetStat(StatType.MaxMP);
            paramValues["stat_minhit"] = (Int16)source.GetStat(StatType.MinHit);
            paramValues["stat_str"] = (Int16)source.GetStat(StatType.Str);
        }

        /// <summary>
        /// Checks if this <see cref="ICharacterTable"/> contains the same values as another <see cref="ICharacterTable"/>.
        /// </summary>
        /// <param name="source">The source <see cref="ICharacterTable"/>.</param>
        /// <param name="otherItem">The <see cref="ICharacterTable"/> to compare the values to.</param>
        /// <returns>
        /// True if this <see cref="ICharacterTable"/> contains the same values as the <paramref name="otherItem"/>; otherwise false.
        /// </returns>
        public static Boolean HasSameValues(this ICharacterTable source, ICharacterTable otherItem)
        {
            return Equals(source.AIID, otherItem.AIID) && Equals(source.BodyID, otherItem.BodyID) &&
                   Equals(source.Cash, otherItem.Cash) && Equals(source.CharacterTemplateID, otherItem.CharacterTemplateID) &&
                   Equals(source.ChatDialog, otherItem.ChatDialog) && Equals(source.Exp, otherItem.Exp) &&
                   Equals(source.HP, otherItem.HP) && Equals(source.ID, otherItem.ID) && Equals(source.Level, otherItem.Level) &&
                   Equals(source.LoadMapID, otherItem.LoadMapID) && Equals(source.LoadX, otherItem.LoadX) &&
                   Equals(source.LoadY, otherItem.LoadY) && Equals(source.MoveSpeed, otherItem.MoveSpeed) &&
                   Equals(source.MP, otherItem.MP) && Equals(source.Name, otherItem.Name) &&
                   Equals(source.RespawnMapID, otherItem.RespawnMapID) && Equals(source.RespawnX, otherItem.RespawnX) &&
                   Equals(source.RespawnY, otherItem.RespawnY) && Equals(source.ShopID, otherItem.ShopID) &&
                   Equals(source.StatPoints, otherItem.StatPoints) &&
                   Equals(source.GetStat(StatType.Agi), otherItem.GetStat(StatType.Agi)) &&
                   Equals(source.GetStat(StatType.Defence), otherItem.GetStat(StatType.Defence)) &&
                   Equals(source.GetStat(StatType.Int), otherItem.GetStat(StatType.Int)) &&
                   Equals(source.GetStat(StatType.MaxHit), otherItem.GetStat(StatType.MaxHit)) &&
                   Equals(source.GetStat(StatType.MaxHP), otherItem.GetStat(StatType.MaxHP)) &&
                   Equals(source.GetStat(StatType.MaxMP), otherItem.GetStat(StatType.MaxMP)) &&
                   Equals(source.GetStat(StatType.MinHit), otherItem.GetStat(StatType.MinHit)) &&
                   Equals(source.GetStat(StatType.Str), otherItem.GetStat(StatType.Str));
        }

        /// <summary>
        /// Reads the values from an <see cref="IDataRecord"/> and assigns the read values to this
        /// object's properties. The database column's name is used to as the key, so the value
        /// will not be found if any aliases are used or not all columns were selected.
        /// </summary>
        /// <param name="source">The object to add the extension method to.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> to read the values from. Must already be ready to be read from.</param>
        public static void ReadValues(this CharacterTable source, IDataRecord dataRecord)
        {
            Int32 i;

            i = dataRecord.GetOrdinal("ai_id");

            source.AIID = (Nullable<AIID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));

            i = dataRecord.GetOrdinal("body_id");

            source.BodyID = (BodyID)dataRecord.GetUInt16(i);

            i = dataRecord.GetOrdinal("cash");

            source.Cash = dataRecord.GetInt32(i);

            i = dataRecord.GetOrdinal("character_template_id");

            source.CharacterTemplateID =
                (Nullable<CharacterTemplateID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));

            i = dataRecord.GetOrdinal("chat_dialog");

            source.ChatDialog = (Nullable<NPCChatDialogID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));

            i = dataRecord.GetOrdinal("exp");

            source.Exp = dataRecord.GetInt32(i);

            i = dataRecord.GetOrdinal("hp");

            source.HP = dataRecord.GetInt16(i);

            i = dataRecord.GetOrdinal("id");

            source.ID = (CharacterID)dataRecord.GetInt32(i);

            i = dataRecord.GetOrdinal("level");

            source.Level = dataRecord.GetByte(i);

            i = dataRecord.GetOrdinal("load_map_id");

            source.LoadMapID = (MapID)dataRecord.GetUInt16(i);

            i = dataRecord.GetOrdinal("load_x");

            source.LoadX = dataRecord.GetUInt16(i);

            i = dataRecord.GetOrdinal("load_y");

            source.LoadY = dataRecord.GetUInt16(i);

            i = dataRecord.GetOrdinal("move_speed");

            source.MoveSpeed = dataRecord.GetUInt16(i);

            i = dataRecord.GetOrdinal("mp");

            source.MP = dataRecord.GetInt16(i);

            i = dataRecord.GetOrdinal("name");

            source.Name = dataRecord.GetString(i);

            i = dataRecord.GetOrdinal("respawn_map_id");

            source.RespawnMapID = (Nullable<MapID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));

            i = dataRecord.GetOrdinal("respawn_x");

            source.RespawnX = dataRecord.GetFloat(i);

            i = dataRecord.GetOrdinal("respawn_y");

            source.RespawnY = dataRecord.GetFloat(i);

            i = dataRecord.GetOrdinal("shop_id");

            source.ShopID = (Nullable<ShopID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));

            i = dataRecord.GetOrdinal("statpoints");

            source.StatPoints = dataRecord.GetInt32(i);

            i = dataRecord.GetOrdinal("stat_agi");

            source.SetStat(StatType.Agi, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_defence");

            source.SetStat(StatType.Defence, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_int");

            source.SetStat(StatType.Int, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_maxhit");

            source.SetStat(StatType.MaxHit, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_maxhp");

            source.SetStat(StatType.MaxHP, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_maxmp");

            source.SetStat(StatType.MaxMP, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_minhit");

            source.SetStat(StatType.MinHit, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_str");

            source.SetStat(StatType.Str, dataRecord.GetInt16(i));
        }

        /// <summary>
        /// Copies the column values into the given DbParameterValues using the database column name
        /// with a prefixed @ as the key. The key must already exist in the DbParameterValues
        /// for the value to be copied over. If any of the keys in the DbParameterValues do not
        /// match one of the column names, or if there is no field for a key, then it will be
        /// ignored. Because of this, it is important to be careful when using this method
        /// since columns or keys can be skipped without any indication.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="paramValues">The DbParameterValues to copy the values into.</param>
        public static void TryCopyValues(this ICharacterTable source, DbParameterValues paramValues)
        {
            for (var i = 0; i < paramValues.Count; i++)
            {
                switch (paramValues.GetParameterName(i))
                {
                    case "ai_id":
                        paramValues[i] = (ushort?)source.AIID;
                        break;

                    case "body_id":
                        paramValues[i] = (UInt16)source.BodyID;
                        break;

                    case "cash":
                        paramValues[i] = source.Cash;
                        break;

                    case "character_template_id":
                        paramValues[i] = (ushort?)source.CharacterTemplateID;
                        break;

                    case "chat_dialog":
                        paramValues[i] = (ushort?)source.ChatDialog;
                        break;

                    case "exp":
                        paramValues[i] = source.Exp;
                        break;

                    case "hp":
                        paramValues[i] = (Int16)source.HP;
                        break;

                    case "id":
                        paramValues[i] = (Int32)source.ID;
                        break;

                    case "level":
                        paramValues[i] = source.Level;
                        break;

                    case "load_map_id":
                        paramValues[i] = (UInt16)source.LoadMapID;
                        break;

                    case "load_x":
                        paramValues[i] = source.LoadX;
                        break;

                    case "load_y":
                        paramValues[i] = source.LoadY;
                        break;

                    case "move_speed":
                        paramValues[i] = source.MoveSpeed;
                        break;

                    case "mp":
                        paramValues[i] = (Int16)source.MP;
                        break;

                    case "name":
                        paramValues[i] = source.Name;
                        break;

                    case "respawn_map_id":
                        paramValues[i] = (ushort?)source.RespawnMapID;
                        break;

                    case "respawn_x":
                        paramValues[i] = source.RespawnX;
                        break;

                    case "respawn_y":
                        paramValues[i] = source.RespawnY;
                        break;

                    case "shop_id":
                        paramValues[i] = (ushort?)source.ShopID;
                        break;

                    case "statpoints":
                        paramValues[i] = source.StatPoints;
                        break;

                    case "stat_agi":
                        paramValues[i] = (Int16)source.GetStat(StatType.Agi);
                        break;

                    case "stat_defence":
                        paramValues[i] = (Int16)source.GetStat(StatType.Defence);
                        break;

                    case "stat_int":
                        paramValues[i] = (Int16)source.GetStat(StatType.Int);
                        break;

                    case "stat_maxhit":
                        paramValues[i] = (Int16)source.GetStat(StatType.MaxHit);
                        break;

                    case "stat_maxhp":
                        paramValues[i] = (Int16)source.GetStat(StatType.MaxHP);
                        break;

                    case "stat_maxmp":
                        paramValues[i] = (Int16)source.GetStat(StatType.MaxMP);
                        break;

                    case "stat_minhit":
                        paramValues[i] = (Int16)source.GetStat(StatType.MinHit);
                        break;

                    case "stat_str":
                        paramValues[i] = (Int16)source.GetStat(StatType.Str);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the values from an <see cref="IDataReader"/> and assigns the read values to this
        /// object's properties. Unlike ReadValues(), this method not only doesn't require
        /// all values to be in the <see cref="IDataReader"/>, but also does not require the values in
        /// the <see cref="IDataReader"/> to be a defined field for the table this class represents.
        /// Because of this, you need to be careful when using this method because values
        /// can easily be skipped without any indication.
        /// </summary>
        /// <param name="source">The object to add the extension method to.</param>
        /// <param name="dataRecord">The <see cref="IDataReader"/> to read the values from. Must already be ready to be read from.</param>
        public static void TryReadValues(this CharacterTable source, IDataRecord dataRecord)
        {
            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                switch (dataRecord.GetName(i))
                {
                    case "ai_id":
                        source.AIID = (Nullable<AIID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));
                        break;

                    case "body_id":
                        source.BodyID = (BodyID)dataRecord.GetUInt16(i);
                        break;

                    case "cash":
                        source.Cash = dataRecord.GetInt32(i);
                        break;

                    case "character_template_id":
                        source.CharacterTemplateID =
                            (Nullable<CharacterTemplateID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));
                        break;

                    case "chat_dialog":
                        source.ChatDialog =
                            (Nullable<NPCChatDialogID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));
                        break;

                    case "exp":
                        source.Exp = dataRecord.GetInt32(i);
                        break;

                    case "hp":
                        source.HP = dataRecord.GetInt16(i);
                        break;

                    case "id":
                        source.ID = (CharacterID)dataRecord.GetInt32(i);
                        break;

                    case "level":
                        source.Level = dataRecord.GetByte(i);
                        break;

                    case "load_map_id":
                        source.LoadMapID = (MapID)dataRecord.GetUInt16(i);
                        break;

                    case "load_x":
                        source.LoadX = dataRecord.GetUInt16(i);
                        break;

                    case "load_y":
                        source.LoadY = dataRecord.GetUInt16(i);
                        break;

                    case "move_speed":
                        source.MoveSpeed = dataRecord.GetUInt16(i);
                        break;

                    case "mp":
                        source.MP = dataRecord.GetInt16(i);
                        break;

                    case "name":
                        source.Name = dataRecord.GetString(i);
                        break;

                    case "respawn_map_id":
                        source.RespawnMapID = (Nullable<MapID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));
                        break;

                    case "respawn_x":
                        source.RespawnX = dataRecord.GetFloat(i);
                        break;

                    case "respawn_y":
                        source.RespawnY = dataRecord.GetFloat(i);
                        break;

                    case "shop_id":
                        source.ShopID = (Nullable<ShopID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));
                        break;

                    case "statpoints":
                        source.StatPoints = dataRecord.GetInt32(i);
                        break;

                    case "stat_agi":
                        source.SetStat(StatType.Agi, dataRecord.GetInt16(i));
                        break;

                    case "stat_defence":
                        source.SetStat(StatType.Defence, dataRecord.GetInt16(i));
                        break;

                    case "stat_int":
                        source.SetStat(StatType.Int, dataRecord.GetInt16(i));
                        break;

                    case "stat_maxhit":
                        source.SetStat(StatType.MaxHit, dataRecord.GetInt16(i));
                        break;

                    case "stat_maxhp":
                        source.SetStat(StatType.MaxHP, dataRecord.GetInt16(i));
                        break;

                    case "stat_maxmp":
                        source.SetStat(StatType.MaxMP, dataRecord.GetInt16(i));
                        break;

                    case "stat_minhit":
                        source.SetStat(StatType.MinHit, dataRecord.GetInt16(i));
                        break;

                    case "stat_str":
                        source.SetStat(StatType.Str, dataRecord.GetInt16(i));
                        break;
                }
            }
        }
    }
}