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
using NetGore;
using NetGore.Db;
using NetGore.Features.ActionDisplays;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Contains extension methods for class ItemTemplateTable that assist in performing
    /// reads and writes to and from a database.
    /// </summary>
    public static class ItemTemplateTableDbExtensions
    {
        /// <summary>
        /// Copies the column values into the given DbParameterValues using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
        ///  this method will not create them if they are missing.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="paramValues">The DbParameterValues to copy the values into.</param>
        public static void CopyValues(this IItemTemplateTable source, DbParameterValues paramValues)
        {
            paramValues["action_display_id"] = (ushort?)source.ActionDisplayID;
            paramValues["description"] = source.Description;
            paramValues["equipped_body"] = source.EquippedBody;
            paramValues["graphic"] = (UInt16)source.Graphic;
            paramValues["height"] = source.Height;
            paramValues["hp"] = (Int16)source.HP;
            paramValues["id"] = (UInt16)source.ID;
            paramValues["mp"] = (Int16)source.MP;
            paramValues["name"] = source.Name;
            paramValues["range"] = source.Range;
            paramValues["stat_agi"] = (Int16)source.GetStat(StatType.Agi);
            paramValues["stat_defence"] = (Int16)source.GetStat(StatType.Defence);
            paramValues["stat_int"] = (Int16)source.GetStat(StatType.Int);
            paramValues["stat_maxhit"] = (Int16)source.GetStat(StatType.MaxHit);
            paramValues["stat_maxhp"] = (Int16)source.GetStat(StatType.MaxHP);
            paramValues["stat_maxmp"] = (Int16)source.GetStat(StatType.MaxMP);
            paramValues["stat_minhit"] = (Int16)source.GetStat(StatType.MinHit);
            paramValues["stat_req_agi"] = (Int16)source.GetReqStat(StatType.Agi);
            paramValues["stat_req_int"] = (Int16)source.GetReqStat(StatType.Int);
            paramValues["stat_req_str"] = (Int16)source.GetReqStat(StatType.Str);
            paramValues["stat_str"] = (Int16)source.GetStat(StatType.Str);
            paramValues["type"] = (Byte)source.Type;
            paramValues["value"] = source.Value;
            paramValues["weapon_type"] = (Byte)source.WeaponType;
            paramValues["width"] = source.Width;
        }

        /// <summary>
        /// Checks if this <see cref="IItemTemplateTable"/> contains the same values as another <see cref="IItemTemplateTable"/>.
        /// </summary>
        /// <param name="source">The source <see cref="IItemTemplateTable"/>.</param>
        /// <param name="otherItem">The <see cref="IItemTemplateTable"/> to compare the values to.</param>
        /// <returns>
        /// True if this <see cref="IItemTemplateTable"/> contains the same values as the <paramref name="otherItem"/>; otherwise false.
        /// </returns>
        public static Boolean HasSameValues(this IItemTemplateTable source, IItemTemplateTable otherItem)
        {
            return Equals(source.ActionDisplayID, otherItem.ActionDisplayID) && Equals(source.Description, otherItem.Description) &&
                   Equals(source.EquippedBody, otherItem.EquippedBody) && Equals(source.Graphic, otherItem.Graphic) &&
                   Equals(source.Height, otherItem.Height) && Equals(source.HP, otherItem.HP) && Equals(source.ID, otherItem.ID) &&
                   Equals(source.MP, otherItem.MP) && Equals(source.Name, otherItem.Name) && Equals(source.Range, otherItem.Range) &&
                   Equals(source.GetStat(StatType.Agi), otherItem.GetStat(StatType.Agi)) &&
                   Equals(source.GetStat(StatType.Defence), otherItem.GetStat(StatType.Defence)) &&
                   Equals(source.GetStat(StatType.Int), otherItem.GetStat(StatType.Int)) &&
                   Equals(source.GetStat(StatType.MaxHit), otherItem.GetStat(StatType.MaxHit)) &&
                   Equals(source.GetStat(StatType.MaxHP), otherItem.GetStat(StatType.MaxHP)) &&
                   Equals(source.GetStat(StatType.MaxMP), otherItem.GetStat(StatType.MaxMP)) &&
                   Equals(source.GetStat(StatType.MinHit), otherItem.GetStat(StatType.MinHit)) &&
                   Equals(source.GetReqStat(StatType.Agi), otherItem.GetReqStat(StatType.Agi)) &&
                   Equals(source.GetReqStat(StatType.Int), otherItem.GetReqStat(StatType.Int)) &&
                   Equals(source.GetReqStat(StatType.Str), otherItem.GetReqStat(StatType.Str)) &&
                   Equals(source.GetStat(StatType.Str), otherItem.GetStat(StatType.Str)) && Equals(source.Type, otherItem.Type) &&
                   Equals(source.Value, otherItem.Value) && Equals(source.WeaponType, otherItem.WeaponType) &&
                   Equals(source.Width, otherItem.Width);
        }

        /// <summary>
        /// Reads the values from an <see cref="IDataRecord"/> and assigns the read values to this
        /// object's properties. The database column's name is used to as the key, so the value
        /// will not be found if any aliases are used or not all columns were selected.
        /// </summary>
        /// <param name="source">The object to add the extension method to.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> to read the values from. Must already be ready to be read from.</param>
        public static void ReadValues(this ItemTemplateTable source, IDataRecord dataRecord)
        {
            Int32 i;

            i = dataRecord.GetOrdinal("action_display_id");

            source.ActionDisplayID = (Nullable<ActionDisplayID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));

            i = dataRecord.GetOrdinal("description");

            source.Description = dataRecord.GetString(i);

            i = dataRecord.GetOrdinal("equipped_body");

            source.EquippedBody = (dataRecord.IsDBNull(i) ? null : dataRecord.GetString(i));

            i = dataRecord.GetOrdinal("graphic");

            source.Graphic = (GrhIndex)dataRecord.GetUInt16(i);

            i = dataRecord.GetOrdinal("height");

            source.Height = dataRecord.GetByte(i);

            i = dataRecord.GetOrdinal("hp");

            source.HP = dataRecord.GetInt16(i);

            i = dataRecord.GetOrdinal("id");

            source.ID = (ItemTemplateID)dataRecord.GetUInt16(i);

            i = dataRecord.GetOrdinal("mp");

            source.MP = dataRecord.GetInt16(i);

            i = dataRecord.GetOrdinal("name");

            source.Name = dataRecord.GetString(i);

            i = dataRecord.GetOrdinal("range");

            source.Range = dataRecord.GetUInt16(i);

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

            i = dataRecord.GetOrdinal("stat_req_agi");

            source.SetReqStat(StatType.Agi, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_req_int");

            source.SetReqStat(StatType.Int, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_req_str");

            source.SetReqStat(StatType.Str, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("stat_str");

            source.SetStat(StatType.Str, dataRecord.GetInt16(i));

            i = dataRecord.GetOrdinal("type");

            source.Type = (ItemType)dataRecord.GetByte(i);

            i = dataRecord.GetOrdinal("value");

            source.Value = dataRecord.GetInt32(i);

            i = dataRecord.GetOrdinal("weapon_type");

            source.WeaponType = (WeaponType)dataRecord.GetByte(i);

            i = dataRecord.GetOrdinal("width");

            source.Width = dataRecord.GetByte(i);
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
        public static void TryCopyValues(this IItemTemplateTable source, DbParameterValues paramValues)
        {
            for (var i = 0; i < paramValues.Count; i++)
            {
                switch (paramValues.GetParameterName(i))
                {
                    case "action_display_id":
                        paramValues[i] = (ushort?)source.ActionDisplayID;
                        break;

                    case "description":
                        paramValues[i] = source.Description;
                        break;

                    case "equipped_body":
                        paramValues[i] = source.EquippedBody;
                        break;

                    case "graphic":
                        paramValues[i] = (UInt16)source.Graphic;
                        break;

                    case "height":
                        paramValues[i] = source.Height;
                        break;

                    case "hp":
                        paramValues[i] = (Int16)source.HP;
                        break;

                    case "id":
                        paramValues[i] = (UInt16)source.ID;
                        break;

                    case "mp":
                        paramValues[i] = (Int16)source.MP;
                        break;

                    case "name":
                        paramValues[i] = source.Name;
                        break;

                    case "range":
                        paramValues[i] = source.Range;
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

                    case "stat_req_agi":
                        paramValues[i] = (Int16)source.GetReqStat(StatType.Agi);
                        break;

                    case "stat_req_int":
                        paramValues[i] = (Int16)source.GetReqStat(StatType.Int);
                        break;

                    case "stat_req_str":
                        paramValues[i] = (Int16)source.GetReqStat(StatType.Str);
                        break;

                    case "stat_str":
                        paramValues[i] = (Int16)source.GetStat(StatType.Str);
                        break;

                    case "type":
                        paramValues[i] = (Byte)source.Type;
                        break;

                    case "value":
                        paramValues[i] = source.Value;
                        break;

                    case "weapon_type":
                        paramValues[i] = (Byte)source.WeaponType;
                        break;

                    case "width":
                        paramValues[i] = source.Width;
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
        public static void TryReadValues(this ItemTemplateTable source, IDataRecord dataRecord)
        {
            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                switch (dataRecord.GetName(i))
                {
                    case "action_display_id":
                        source.ActionDisplayID =
                            (Nullable<ActionDisplayID>)(dataRecord.IsDBNull(i) ? (ushort?)null : dataRecord.GetUInt16(i));
                        break;

                    case "description":
                        source.Description = dataRecord.GetString(i);
                        break;

                    case "equipped_body":
                        source.EquippedBody = (dataRecord.IsDBNull(i) ? null : dataRecord.GetString(i));
                        break;

                    case "graphic":
                        source.Graphic = (GrhIndex)dataRecord.GetUInt16(i);
                        break;

                    case "height":
                        source.Height = dataRecord.GetByte(i);
                        break;

                    case "hp":
                        source.HP = dataRecord.GetInt16(i);
                        break;

                    case "id":
                        source.ID = (ItemTemplateID)dataRecord.GetUInt16(i);
                        break;

                    case "mp":
                        source.MP = dataRecord.GetInt16(i);
                        break;

                    case "name":
                        source.Name = dataRecord.GetString(i);
                        break;

                    case "range":
                        source.Range = dataRecord.GetUInt16(i);
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

                    case "stat_req_agi":
                        source.SetReqStat(StatType.Agi, dataRecord.GetInt16(i));
                        break;

                    case "stat_req_int":
                        source.SetReqStat(StatType.Int, dataRecord.GetInt16(i));
                        break;

                    case "stat_req_str":
                        source.SetReqStat(StatType.Str, dataRecord.GetInt16(i));
                        break;

                    case "stat_str":
                        source.SetStat(StatType.Str, dataRecord.GetInt16(i));
                        break;

                    case "type":
                        source.Type = (ItemType)dataRecord.GetByte(i);
                        break;

                    case "value":
                        source.Value = dataRecord.GetInt32(i);
                        break;

                    case "weapon_type":
                        source.WeaponType = (WeaponType)dataRecord.GetByte(i);
                        break;

                    case "width":
                        source.Width = dataRecord.GetByte(i);
                        break;
                }
            }
        }
    }
}