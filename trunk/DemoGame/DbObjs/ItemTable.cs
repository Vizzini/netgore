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
using System.ComponentModel;
using System.Linq;
using NetGore;
using NetGore.Features.ActionDisplays;
using NetGore.IO;

namespace DemoGame.DbObjs
{
    /// <summary>
    /// Provides a strongly-typed structure for the database table `item`.
    /// </summary>
    public class ItemTable : IItemTable, IPersistable
    {
        /// <summary>
        /// The number of columns in the database table that this class represents.
        /// </summary>
        public const Int32 ColumnCount = 27;

        /// <summary>
        /// The name of the database table that this class represents.
        /// </summary>
        public const String TableName = "item";

        /// <summary>
        /// Array of the database column names.
        /// </summary>
        static readonly String[] _dbColumns = new string[]
        {
            "action_display_id", "amount", "description", "equipped_body", "graphic", "height", "hp", "id", "item_template_id", "mp"
            , "name", "range", "stat_agi", "stat_defence", "stat_int", "stat_maxhit", "stat_maxhp", "stat_maxmp", "stat_minhit",
            "stat_req_agi", "stat_req_int", "stat_req_str", "stat_str", "type", "value", "weapon_type", "width"
        };

        /// <summary>
        /// Array of the database column names for columns that are primary keys.
        /// </summary>
        static readonly String[] _dbColumnsKeys = new string[] { "id" };

        /// <summary>
        /// Array of the database column names for columns that are not primary keys.
        /// </summary>
        static readonly String[] _dbColumnsNonKey = new string[]
        {
            "action_display_id", "amount", "description", "equipped_body", "graphic", "height", "hp", "item_template_id", "mp",
            "name", "range", "stat_agi", "stat_defence", "stat_int", "stat_maxhit", "stat_maxhp", "stat_maxmp", "stat_minhit",
            "stat_req_agi", "stat_req_int", "stat_req_str", "stat_str", "type", "value", "weapon_type", "width"
        };

        /// <summary>
        /// The fields that are used in the column collection `ReqStat`.
        /// </summary>
        static readonly String[] _reqStatColumns = new string[] { "stat_req_agi", "stat_req_int", "stat_req_str" };

        /// <summary>
        /// The fields that are used in the column collection `Stat`.
        /// </summary>
        static readonly String[] _statColumns = new string[]
        { "stat_agi", "stat_defence", "stat_int", "stat_maxhit", "stat_maxhp", "stat_maxmp", "stat_minhit", "stat_str" };

        /// <summary>
        /// Dictionary containing the values for the column collection `ReqStat`.
        /// </summary>
        readonly StatTypeConstDictionary _reqStat = new StatTypeConstDictionary();

        /// <summary>
        /// Dictionary containing the values for the column collection `Stat`.
        /// </summary>
        readonly StatTypeConstDictionary _stat = new StatTypeConstDictionary();

        /// <summary>
        /// The field that maps onto the database column `action_display_id`.
        /// </summary>
        ushort? _actionDisplayID;

        /// <summary>
        /// The field that maps onto the database column `amount`.
        /// </summary>
        Byte _amount;

        /// <summary>
        /// The field that maps onto the database column `description`.
        /// </summary>
        String _description;

        /// <summary>
        /// The field that maps onto the database column `equipped_body`.
        /// </summary>
        String _equippedBody;

        /// <summary>
        /// The field that maps onto the database column `graphic`.
        /// </summary>
        UInt16 _graphic;

        /// <summary>
        /// The field that maps onto the database column `hp`.
        /// </summary>
        Int16 _hP;

        /// <summary>
        /// The field that maps onto the database column `height`.
        /// </summary>
        Byte _height;

        /// <summary>
        /// The field that maps onto the database column `id`.
        /// </summary>
        Int32 _iD;

        /// <summary>
        /// The field that maps onto the database column `item_template_id`.
        /// </summary>
        ushort? _itemTemplateID;

        /// <summary>
        /// The field that maps onto the database column `mp`.
        /// </summary>
        Int16 _mP;

        /// <summary>
        /// The field that maps onto the database column `name`.
        /// </summary>
        String _name;

        /// <summary>
        /// The field that maps onto the database column `range`.
        /// </summary>
        UInt16 _range;

        /// <summary>
        /// The field that maps onto the database column `type`.
        /// </summary>
        Byte _type;

        /// <summary>
        /// The field that maps onto the database column `value`.
        /// </summary>
        Int32 _value;

        /// <summary>
        /// The field that maps onto the database column `weapon_type`.
        /// </summary>
        Byte _weaponType;

        /// <summary>
        /// The field that maps onto the database column `width`.
        /// </summary>
        Byte _width;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTable"/> class.
        /// </summary>
        public ItemTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTable"/> class.
        /// </summary>
        /// <param name="actionDisplayID">The initial value for the corresponding property.</param>
        /// <param name="amount">The initial value for the corresponding property.</param>
        /// <param name="description">The initial value for the corresponding property.</param>
        /// <param name="equippedBody">The initial value for the corresponding property.</param>
        /// <param name="graphic">The initial value for the corresponding property.</param>
        /// <param name="height">The initial value for the corresponding property.</param>
        /// <param name="hP">The initial value for the corresponding property.</param>
        /// <param name="iD">The initial value for the corresponding property.</param>
        /// <param name="itemTemplateID">The initial value for the corresponding property.</param>
        /// <param name="mP">The initial value for the corresponding property.</param>
        /// <param name="name">The initial value for the corresponding property.</param>
        /// <param name="range">The initial value for the corresponding property.</param>
        /// <param name="statAgi">The initial value for the corresponding property.</param>
        /// <param name="statDefence">The initial value for the corresponding property.</param>
        /// <param name="statInt">The initial value for the corresponding property.</param>
        /// <param name="statMaxhit">The initial value for the corresponding property.</param>
        /// <param name="statMaxhp">The initial value for the corresponding property.</param>
        /// <param name="statMaxmp">The initial value for the corresponding property.</param>
        /// <param name="statMinhit">The initial value for the corresponding property.</param>
        /// <param name="statReqAgi">The initial value for the corresponding property.</param>
        /// <param name="statReqInt">The initial value for the corresponding property.</param>
        /// <param name="statReqStr">The initial value for the corresponding property.</param>
        /// <param name="statStr">The initial value for the corresponding property.</param>
        /// <param name="type">The initial value for the corresponding property.</param>
        /// <param name="value">The initial value for the corresponding property.</param>
        /// <param name="weaponType">The initial value for the corresponding property.</param>
        /// <param name="width">The initial value for the corresponding property.</param>
        public ItemTable(ActionDisplayID? @actionDisplayID, Byte @amount, String @description, String @equippedBody,
                         GrhIndex @graphic, Byte @height, SPValueType @hP, ItemID @iD, ItemTemplateID? @itemTemplateID,
                         SPValueType @mP, String @name, UInt16 @range, Int16 @statAgi, Int16 @statDefence, Int16 @statInt,
                         Int16 @statMaxhit, Int16 @statMaxhp, Int16 @statMaxmp, Int16 @statMinhit, Int16 @statReqAgi,
                         Int16 @statReqInt, Int16 @statReqStr, Int16 @statStr, ItemType @type, Int32 @value,
                         WeaponType @weaponType, Byte @width)
        {
            ActionDisplayID = @actionDisplayID;
            Amount = @amount;
            Description = @description;
            EquippedBody = @equippedBody;
            Graphic = @graphic;
            Height = @height;
            HP = @hP;
            ID = @iD;
            ItemTemplateID = @itemTemplateID;
            MP = @mP;
            Name = @name;
            Range = @range;
            SetStat(StatType.Agi, @statAgi);
            SetStat(StatType.Defence, @statDefence);
            SetStat(StatType.Int, @statInt);
            SetStat(StatType.MaxHit, @statMaxhit);
            SetStat(StatType.MaxHP, @statMaxhp);
            SetStat(StatType.MaxMP, @statMaxmp);
            SetStat(StatType.MinHit, @statMinhit);
            SetReqStat(StatType.Agi, @statReqAgi);
            SetReqStat(StatType.Int, @statReqInt);
            SetReqStat(StatType.Str, @statReqStr);
            SetStat(StatType.Str, @statStr);
            Type = @type;
            Value = @value;
            WeaponType = @weaponType;
            Width = @width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTable"/> class.
        /// </summary>
        /// <param name="source">IItemTable to copy the initial values from.</param>
        public ItemTable(IItemTable source)
        {
            CopyValuesFrom(source);
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns for the table that this class represents.
        /// </summary>
        public static IEnumerable<String> DbColumns
        {
            get { return _dbColumns; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns that are primary keys.
        /// </summary>
        public static IEnumerable<String> DbKeyColumns
        {
            get { return _dbColumnsKeys; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns that are not primary keys.
        /// </summary>
        public static IEnumerable<String> DbNonKeyColumns
        {
            get { return _dbColumnsNonKey; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the name of the database
        /// columns used in the column collection `ReqStat`.
        /// </summary>
        public static IEnumerable<String> ReqStatColumns
        {
            get { return _reqStatColumns; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the name of the database
        /// columns used in the column collection `Stat`.
        /// </summary>
        public static IEnumerable<String> StatColumns
        {
            get { return _statColumns; }
        }

        /// <summary>
        /// Copies the column values into the given Dictionary using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the Dictionary;
        /// this method will not create them if they are missing.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="dic">The Dictionary to copy the values into.</param>
        public static void CopyValues(IItemTable source, IDictionary<String, Object> dic)
        {
            dic["action_display_id"] = source.ActionDisplayID;
            dic["amount"] = source.Amount;
            dic["description"] = source.Description;
            dic["equipped_body"] = source.EquippedBody;
            dic["graphic"] = source.Graphic;
            dic["height"] = source.Height;
            dic["hp"] = source.HP;
            dic["id"] = source.ID;
            dic["item_template_id"] = source.ItemTemplateID;
            dic["mp"] = source.MP;
            dic["name"] = source.Name;
            dic["range"] = source.Range;
            dic["stat_agi"] = (Int16)source.GetStat(StatType.Agi);
            dic["stat_defence"] = (Int16)source.GetStat(StatType.Defence);
            dic["stat_int"] = (Int16)source.GetStat(StatType.Int);
            dic["stat_maxhit"] = (Int16)source.GetStat(StatType.MaxHit);
            dic["stat_maxhp"] = (Int16)source.GetStat(StatType.MaxHP);
            dic["stat_maxmp"] = (Int16)source.GetStat(StatType.MaxMP);
            dic["stat_minhit"] = (Int16)source.GetStat(StatType.MinHit);
            dic["stat_req_agi"] = (Int16)source.GetReqStat(StatType.Agi);
            dic["stat_req_int"] = (Int16)source.GetReqStat(StatType.Int);
            dic["stat_req_str"] = (Int16)source.GetReqStat(StatType.Str);
            dic["stat_str"] = (Int16)source.GetStat(StatType.Str);
            dic["type"] = source.Type;
            dic["value"] = source.Value;
            dic["weapon_type"] = source.WeaponType;
            dic["width"] = source.Width;
        }

        /// <summary>
        /// Copies the column values into the given Dictionary using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the Dictionary;
        /// this method will not create them if they are missing.
        /// </summary>
        /// <param name="dic">The Dictionary to copy the values into.</param>
        public void CopyValues(IDictionary<String, Object> dic)
        {
            CopyValues(this, dic);
        }

        /// <summary>
        /// Copies the values from the given <paramref name="source"/> into this ItemTable.
        /// </summary>
        /// <param name="source">The IItemTable to copy the values from.</param>
        public void CopyValuesFrom(IItemTable source)
        {
            ActionDisplayID = source.ActionDisplayID;
            Amount = source.Amount;
            Description = source.Description;
            EquippedBody = source.EquippedBody;
            Graphic = source.Graphic;
            Height = source.Height;
            HP = source.HP;
            ID = source.ID;
            ItemTemplateID = source.ItemTemplateID;
            MP = source.MP;
            Name = source.Name;
            Range = source.Range;
            SetStat(StatType.Agi, source.GetStat(StatType.Agi));
            SetStat(StatType.Defence, source.GetStat(StatType.Defence));
            SetStat(StatType.Int, source.GetStat(StatType.Int));
            SetStat(StatType.MaxHit, source.GetStat(StatType.MaxHit));
            SetStat(StatType.MaxHP, source.GetStat(StatType.MaxHP));
            SetStat(StatType.MaxMP, source.GetStat(StatType.MaxMP));
            SetStat(StatType.MinHit, source.GetStat(StatType.MinHit));
            SetReqStat(StatType.Agi, source.GetReqStat(StatType.Agi));
            SetReqStat(StatType.Int, source.GetReqStat(StatType.Int));
            SetReqStat(StatType.Str, source.GetReqStat(StatType.Str));
            SetStat(StatType.Str, source.GetStat(StatType.Str));
            Type = source.Type;
            Value = source.Value;
            WeaponType = source.WeaponType;
            Width = source.Width;
        }

        /// <summary>
        /// Gets the data for the database column that this table represents.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the data for.</param>
        /// <returns>
        /// The data for the database column with the name <paramref name="columnName"/>.
        /// </returns>
        public static ColumnMetadata GetColumnData(String columnName)
        {
            switch (columnName)
            {
                case "action_display_id":
                    return new ColumnMetadata("action_display_id",
                        "The ActionDisplayID to use when using this item (e.g. drink potion, attack with sword, etc).",
                        "smallint(5) unsigned", null, typeof(ushort?), true, false, false);

                case "amount":
                    return new ColumnMetadata("amount",
                        "The quantity of the item (for stacked items). Stacks of items count as one single item instance with an amount greater than zero.",
                        "tinyint(3) unsigned", "1", typeof(Byte), false, false, false);

                case "description":
                    return new ColumnMetadata("description", "The item's textual description (don't include stuff like stats).",
                        "varchar(255)", null, typeof(String), false, false, false);

                case "equipped_body":
                    return new ColumnMetadata("equipped_body",
                        "When equipped and not null, sets the character's paper doll to include this layer.", "varchar(255)", null,
                        typeof(String), true, false, false);

                case "graphic":
                    return new ColumnMetadata("graphic",
                        "The GrhData to use to display this item, both in GUI (inventory, equipped) and on the map.",
                        "smallint(5) unsigned", "0", typeof(UInt16), false, false, false);

                case "height":
                    return new ColumnMetadata("height",
                        "Height of the item in pixels. Mostly intended for when on a map. Usually set to the same size as the item's sprite.",
                        "tinyint(3) unsigned", "16", typeof(Byte), false, false, false);

                case "hp":
                    return new ColumnMetadata("hp", "Amount of health gained from using this item (mostly for use-once items).",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "id":
                    return new ColumnMetadata("id", "The unique ID of the item.", "int(11)", null, typeof(Int32), false, true,
                        false);

                case "item_template_id":
                    return new ColumnMetadata("item_template_id",
                        "The template the item was created from. Not required. Mostly for development reference.",
                        "smallint(5) unsigned", null, typeof(ushort?), true, false, true);

                case "mp":
                    return new ColumnMetadata("mp", "Amount of mana gained from using this item (mostly for use-once items).",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "name":
                    return new ColumnMetadata("name", "The name of the item.", "varchar(255)", null, typeof(String), false, false,
                        false);

                case "range":
                    return new ColumnMetadata("range",
                        "The range of the item. Usually for attack range, but can depend on ItemType and/or WeaponType.",
                        "smallint(5) unsigned", null, typeof(UInt16), false, false, false);

                case "stat_agi":
                    return new ColumnMetadata("stat_agi",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_defence":
                    return new ColumnMetadata("stat_defence",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_int":
                    return new ColumnMetadata("stat_int",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_maxhit":
                    return new ColumnMetadata("stat_maxhit",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_maxhp":
                    return new ColumnMetadata("stat_maxhp",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_maxmp":
                    return new ColumnMetadata("stat_maxmp",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_minhit":
                    return new ColumnMetadata("stat_minhit",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_req_agi":
                    return new ColumnMetadata("stat_req_agi", "Required amount of the corresponding stat to use this item.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_req_int":
                    return new ColumnMetadata("stat_req_int", "Required amount of the corresponding stat to use this item.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_req_str":
                    return new ColumnMetadata("stat_req_str", "Required amount of the corresponding stat to use this item.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "stat_str":
                    return new ColumnMetadata("stat_str",
                        "Stat modifier bonus. Use-once items often perminately increase this value, while equipped items provide a stat mod bonus.",
                        "smallint(6)", "0", typeof(Int16), false, false, false);

                case "type":
                    return new ColumnMetadata("type", "The type of item (see ItemType enum).", "tinyint(3) unsigned", "0",
                        typeof(Byte), false, false, false);

                case "value":
                    return new ColumnMetadata("value", "The base monetary value of the item.", "int(11)", "0", typeof(Int32),
                        false, false, false);

                case "weapon_type":
                    return new ColumnMetadata("weapon_type", "When used as a weapon, the type of weapon (see WeaponType enum).",
                        "tinyint(3) unsigned", null, typeof(Byte), false, false, false);

                case "width":
                    return new ColumnMetadata("width",
                        "Width of the item in pixels. Mostly intended for when on a map. Usually set to the same size as the item's sprite.",
                        "tinyint(3) unsigned", "16", typeof(Byte), false, false, false);

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        /// <summary>
        /// Gets the value of a column by the database column's name.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the value for.</param>
        /// <returns>
        /// The value of the column with the name <paramref name="columnName"/>.
        /// </returns>
        public Object GetValue(String columnName)
        {
            switch (columnName)
            {
                case "action_display_id":
                    return ActionDisplayID;

                case "amount":
                    return Amount;

                case "description":
                    return Description;

                case "equipped_body":
                    return EquippedBody;

                case "graphic":
                    return Graphic;

                case "height":
                    return Height;

                case "hp":
                    return HP;

                case "id":
                    return ID;

                case "item_template_id":
                    return ItemTemplateID;

                case "mp":
                    return MP;

                case "name":
                    return Name;

                case "range":
                    return Range;

                case "stat_agi":
                    return GetStat(StatType.Agi);

                case "stat_defence":
                    return GetStat(StatType.Defence);

                case "stat_int":
                    return GetStat(StatType.Int);

                case "stat_maxhit":
                    return GetStat(StatType.MaxHit);

                case "stat_maxhp":
                    return GetStat(StatType.MaxHP);

                case "stat_maxmp":
                    return GetStat(StatType.MaxMP);

                case "stat_minhit":
                    return GetStat(StatType.MinHit);

                case "stat_req_agi":
                    return GetReqStat(StatType.Agi);

                case "stat_req_int":
                    return GetReqStat(StatType.Int);

                case "stat_req_str":
                    return GetReqStat(StatType.Str);

                case "stat_str":
                    return GetStat(StatType.Str);

                case "type":
                    return Type;

                case "value":
                    return Value;

                case "weapon_type":
                    return WeaponType;

                case "width":
                    return Width;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        /// <summary>
        /// Gets the <paramref name="value"/> of a database column for the corresponding <paramref name="key"/> for the column collection `ReqStat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <param name="value">The value to assign to the column for the corresponding <paramref name="key"/>.</param>
        public void SetReqStat(StatType key, Int32 value)
        {
            _reqStat[key] = (Int16)value;
        }

        /// <summary>
        /// Gets the <paramref name="value"/> of a database column for the corresponding <paramref name="key"/> for the column collection `Stat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <param name="value">The value to assign to the column for the corresponding <paramref name="key"/>.</param>
        public void SetStat(StatType key, Int32 value)
        {
            _stat[key] = (Int16)value;
        }

        /// <summary>
        /// Sets the <paramref name="value"/> of a column by the database column's name.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the <paramref name="value"/> for.</param>
        /// <param name="value">Value to assign to the column.</param>
        public void SetValue(String columnName, Object value)
        {
            switch (columnName)
            {
                case "action_display_id":
                    ActionDisplayID = (ActionDisplayID?)value;
                    break;

                case "amount":
                    Amount = (Byte)value;
                    break;

                case "description":
                    Description = (String)value;
                    break;

                case "equipped_body":
                    EquippedBody = (String)value;
                    break;

                case "graphic":
                    Graphic = (GrhIndex)value;
                    break;

                case "height":
                    Height = (Byte)value;
                    break;

                case "hp":
                    HP = (SPValueType)value;
                    break;

                case "id":
                    ID = (ItemID)value;
                    break;

                case "item_template_id":
                    ItemTemplateID = (ItemTemplateID?)value;
                    break;

                case "mp":
                    MP = (SPValueType)value;
                    break;

                case "name":
                    Name = (String)value;
                    break;

                case "range":
                    Range = (UInt16)value;
                    break;

                case "stat_agi":
                    SetStat(StatType.Agi, (Int32)value);
                    break;

                case "stat_defence":
                    SetStat(StatType.Defence, (Int32)value);
                    break;

                case "stat_int":
                    SetStat(StatType.Int, (Int32)value);
                    break;

                case "stat_maxhit":
                    SetStat(StatType.MaxHit, (Int32)value);
                    break;

                case "stat_maxhp":
                    SetStat(StatType.MaxHP, (Int32)value);
                    break;

                case "stat_maxmp":
                    SetStat(StatType.MaxMP, (Int32)value);
                    break;

                case "stat_minhit":
                    SetStat(StatType.MinHit, (Int32)value);
                    break;

                case "stat_req_agi":
                    SetReqStat(StatType.Agi, (Int32)value);
                    break;

                case "stat_req_int":
                    SetReqStat(StatType.Int, (Int32)value);
                    break;

                case "stat_req_str":
                    SetReqStat(StatType.Str, (Int32)value);
                    break;

                case "stat_str":
                    SetStat(StatType.Str, (Int32)value);
                    break;

                case "type":
                    Type = (ItemType)value;
                    break;

                case "value":
                    Value = (Int32)value;
                    break;

                case "weapon_type":
                    WeaponType = (WeaponType)value;
                    break;

                case "width":
                    Width = (Byte)value;
                    break;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        #region IItemTable Members

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `action_display_id`.
        /// The underlying database type is `smallint(5) unsigned`.The database column contains the comment: 
        /// "The ActionDisplayID to use when using this item (e.g. drink potion, attack with sword, etc).".
        /// </summary>
        [Description("The ActionDisplayID to use when using this item (e.g. drink potion, attack with sword, etc).")]
        [SyncValue]
        public ActionDisplayID? ActionDisplayID
        {
            get { return (Nullable<ActionDisplayID>)_actionDisplayID; }
            set { _actionDisplayID = (ushort?)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `amount`.
        /// The underlying database type is `tinyint(3) unsigned` with the default value of `1`.The database column contains the comment: 
        /// "The quantity of the item (for stacked items). Stacks of items count as one single item instance with an amount greater than zero.".
        /// </summary>
        [Description(
            "The quantity of the item (for stacked items). Stacks of items count as one single item instance with an amount greater than zero."
            )]
        [SyncValue]
        public Byte Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `description`.
        /// The underlying database type is `varchar(255)`.The database column contains the comment: 
        /// "The item's textual description (don't include stuff like stats).".
        /// </summary>
        [Description("The item's textual description (don't include stuff like stats).")]
        [SyncValue]
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `equipped_body`.
        /// The underlying database type is `varchar(255)`.The database column contains the comment: 
        /// "When equipped and not null, sets the character's paper doll to include this layer.".
        /// </summary>
        [Description("When equipped and not null, sets the character's paper doll to include this layer.")]
        [SyncValue]
        public String EquippedBody
        {
            get { return _equippedBody; }
            set { _equippedBody = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `graphic`.
        /// The underlying database type is `smallint(5) unsigned` with the default value of `0`.The database column contains the comment: 
        /// "The GrhData to use to display this item, both in GUI (inventory, equipped) and on the map.".
        /// </summary>
        [Description("The GrhData to use to display this item, both in GUI (inventory, equipped) and on the map.")]
        [SyncValue]
        public GrhIndex Graphic
        {
            get { return (GrhIndex)_graphic; }
            set { _graphic = (UInt16)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `hp`.
        /// The underlying database type is `smallint(6)` with the default value of `0`.The database column contains the comment: 
        /// "Amount of health gained from using this item (mostly for use-once items).".
        /// </summary>
        [Description("Amount of health gained from using this item (mostly for use-once items).")]
        [SyncValue]
        public SPValueType HP
        {
            get { return _hP; }
            set { _hP = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `height`.
        /// The underlying database type is `tinyint(3) unsigned` with the default value of `16`.The database column contains the comment: 
        /// "Height of the item in pixels. Mostly intended for when on a map. Usually set to the same size as the item's sprite.".
        /// </summary>
        [Description(
            "Height of the item in pixels. Mostly intended for when on a map. Usually set to the same size as the item's sprite.")
        ]
        [SyncValue]
        public Byte Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `id`.
        /// The underlying database type is `int(11)`.The database column contains the comment: 
        /// "The unique ID of the item.".
        /// </summary>
        [Description("The unique ID of the item.")]
        [SyncValue]
        public ItemID ID
        {
            get { return (ItemID)_iD; }
            set { _iD = (Int32)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `item_template_id`.
        /// The underlying database type is `smallint(5) unsigned`.The database column contains the comment: 
        /// "The template the item was created from. Not required. Mostly for development reference.".
        /// </summary>
        [Description("The template the item was created from. Not required. Mostly for development reference.")]
        [SyncValue]
        public ItemTemplateID? ItemTemplateID
        {
            get { return (Nullable<ItemTemplateID>)_itemTemplateID; }
            set { _itemTemplateID = (ushort?)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `mp`.
        /// The underlying database type is `smallint(6)` with the default value of `0`.The database column contains the comment: 
        /// "Amount of mana gained from using this item (mostly for use-once items).".
        /// </summary>
        [Description("Amount of mana gained from using this item (mostly for use-once items).")]
        [SyncValue]
        public SPValueType MP
        {
            get { return _mP; }
            set { _mP = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `name`.
        /// The underlying database type is `varchar(255)`.The database column contains the comment: 
        /// "The name of the item.".
        /// </summary>
        [Description("The name of the item.")]
        [SyncValue]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `range`.
        /// The underlying database type is `smallint(5) unsigned`.The database column contains the comment: 
        /// "The range of the item. Usually for attack range, but can depend on ItemType and/or WeaponType.".
        /// </summary>
        [Description("The range of the item. Usually for attack range, but can depend on ItemType and/or WeaponType.")]
        [SyncValue]
        public UInt16 Range
        {
            get { return _range; }
            set { _range = value; }
        }

        /// <summary>
        /// Gets an IEnumerable of KeyValuePairs containing the values in the `ReqStat` collection. The
        /// key is the collection's key and the value is the value for that corresponding key.
        /// </summary>
        public IEnumerable<KeyValuePair<StatType, Int32>> ReqStats
        {
            get { return _reqStat; }
        }

        /// <summary>
        /// Gets an IEnumerable of KeyValuePairs containing the values in the `Stat` collection. The
        /// key is the collection's key and the value is the value for that corresponding key.
        /// </summary>
        public IEnumerable<KeyValuePair<StatType, Int32>> Stats
        {
            get { return _stat; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `type`.
        /// The underlying database type is `tinyint(3) unsigned` with the default value of `0`.The database column contains the comment: 
        /// "The type of item (see ItemType enum).".
        /// </summary>
        [Description("The type of item (see ItemType enum).")]
        [SyncValue]
        public ItemType Type
        {
            get { return (ItemType)_type; }
            set { _type = (Byte)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `value`.
        /// The underlying database type is `int(11)` with the default value of `0`.The database column contains the comment: 
        /// "The base monetary value of the item.".
        /// </summary>
        [Description("The base monetary value of the item.")]
        [SyncValue]
        public Int32 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `weapon_type`.
        /// The underlying database type is `tinyint(3) unsigned`.The database column contains the comment: 
        /// "When used as a weapon, the type of weapon (see WeaponType enum).".
        /// </summary>
        [Description("When used as a weapon, the type of weapon (see WeaponType enum).")]
        [SyncValue]
        public WeaponType WeaponType
        {
            get { return (WeaponType)_weaponType; }
            set { _weaponType = (Byte)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `width`.
        /// The underlying database type is `tinyint(3) unsigned` with the default value of `16`.The database column contains the comment: 
        /// "Width of the item in pixels. Mostly intended for when on a map. Usually set to the same size as the item's sprite.".
        /// </summary>
        [Description(
            "Width of the item in pixels. Mostly intended for when on a map. Usually set to the same size as the item's sprite.")]
        [SyncValue]
        public Byte Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        public virtual IItemTable DeepCopy()
        {
            return new ItemTable(this);
        }

        /// <summary>
        /// Gets the value of a database column for the corresponding <paramref name="key"/> for the column collection `ReqStat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <returns>
        /// The value of the database column for the corresponding <paramref name="key"/>.
        /// </returns>
        public Int32 GetReqStat(StatType key)
        {
            return _reqStat[key];
        }

        /// <summary>
        /// Gets the value of a database column for the corresponding <paramref name="key"/> for the column collection `Stat`.
        /// </summary>
        /// <param name="key">The key of the column to get.</param>
        /// <returns>
        /// The value of the database column for the corresponding <paramref name="key"/>.
        /// </returns>
        public Int32 GetStat(StatType key)
        {
            return _stat[key];
        }

        #endregion

        #region IPersistable Members

        /// <summary>
        /// Reads the state of the object from an <see cref="IValueReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="IValueReader"/> to read the values from.</param>
        public virtual void ReadState(IValueReader reader)
        {
            PersistableHelper.Read(this, reader);
        }

        /// <summary>
        /// Writes the state of the object to an <see cref="IValueWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="IValueWriter"/> to write the values to.</param>
        public virtual void WriteState(IValueWriter writer)
        {
            PersistableHelper.Write(this, writer);
        }

        #endregion
    }
}