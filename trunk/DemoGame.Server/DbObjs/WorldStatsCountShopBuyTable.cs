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
using DemoGame.DbObjs;
using NetGore;
using NetGore.Features.Shops;
using NetGore.IO;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Provides a strongly-typed structure for the database table `world_stats_count_shop_buy`.
    /// </summary>
    public class WorldStatsCountShopBuyTable : IWorldStatsCountShopBuyTable, IPersistable
    {
        /// <summary>
        /// The number of columns in the database table that this class represents.
        /// </summary>
        public const Int32 ColumnCount = 3;

        /// <summary>
        /// The name of the database table that this class represents.
        /// </summary>
        public const String TableName = "world_stats_count_shop_buy";

        /// <summary>
        /// Array of the database column names.
        /// </summary>
        static readonly String[] _dbColumns = new string[] { "count", "last_update", "shop_id" };

        /// <summary>
        /// Array of the database column names for columns that are primary keys.
        /// </summary>
        static readonly String[] _dbColumnsKeys = new string[] { "shop_id" };

        /// <summary>
        /// Array of the database column names for columns that are not primary keys.
        /// </summary>
        static readonly String[] _dbColumnsNonKey = new string[] { "count", "last_update" };

        /// <summary>
        /// The field that maps onto the database column `count`.
        /// </summary>
        Int32 _count;

        /// <summary>
        /// The field that maps onto the database column `last_update`.
        /// </summary>
        DateTime _lastUpdate;

        /// <summary>
        /// The field that maps onto the database column `shop_id`.
        /// </summary>
        UInt16 _shopID;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldStatsCountShopBuyTable"/> class.
        /// </summary>
        public WorldStatsCountShopBuyTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldStatsCountShopBuyTable"/> class.
        /// </summary>
        /// <param name="count">The initial value for the corresponding property.</param>
        /// <param name="lastUpdate">The initial value for the corresponding property.</param>
        /// <param name="shopID">The initial value for the corresponding property.</param>
        public WorldStatsCountShopBuyTable(Int32 @count, DateTime @lastUpdate, ShopID @shopID)
        {
            Count = @count;
            LastUpdate = @lastUpdate;
            ShopID = @shopID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldStatsCountShopBuyTable"/> class.
        /// </summary>
        /// <param name="source">IWorldStatsCountShopBuyTable to copy the initial values from.</param>
        public WorldStatsCountShopBuyTable(IWorldStatsCountShopBuyTable source)
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
        /// Copies the column values into the given Dictionary using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the Dictionary;
        /// this method will not create them if they are missing.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="dic">The Dictionary to copy the values into.</param>
        public static void CopyValues(IWorldStatsCountShopBuyTable source, IDictionary<String, Object> dic)
        {
            dic["count"] = source.Count;
            dic["last_update"] = source.LastUpdate;
            dic["shop_id"] = source.ShopID;
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
        /// Copies the values from the given <paramref name="source"/> into this WorldStatsCountShopBuyTable.
        /// </summary>
        /// <param name="source">The IWorldStatsCountShopBuyTable to copy the values from.</param>
        public void CopyValuesFrom(IWorldStatsCountShopBuyTable source)
        {
            Count = source.Count;
            LastUpdate = source.LastUpdate;
            ShopID = source.ShopID;
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
                case "count":
                    return new ColumnMetadata("count",
                        "The number of times this shop has sold (shopper has bought from this shop).", "int(11)", "0",
                        typeof(Int32), false, false, false);

                case "last_update":
                    return new ColumnMetadata("last_update", "When this counter was last updated.", "timestamp",
                        "CURRENT_TIMESTAMP", typeof(DateTime), false, false, false);

                case "shop_id":
                    return new ColumnMetadata("shop_id", "The shop this counter is for.", "smallint(5) unsigned", null,
                        typeof(UInt16), false, true, false);

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
                case "count":
                    return Count;

                case "last_update":
                    return LastUpdate;

                case "shop_id":
                    return ShopID;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
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
                case "count":
                    Count = (Int32)value;
                    break;

                case "last_update":
                    LastUpdate = (DateTime)value;
                    break;

                case "shop_id":
                    ShopID = (ShopID)value;
                    break;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

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

        #region IWorldStatsCountShopBuyTable Members

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `count`.
        /// The underlying database type is `int(11)` with the default value of `0`.The database column contains the comment: 
        /// "The number of times this shop has sold (shopper has bought from this shop).".
        /// </summary>
        [Description("The number of times this shop has sold (shopper has bought from this shop).")]
        [SyncValue]
        public Int32 Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `last_update`.
        /// The underlying database type is `timestamp` with the default value of `CURRENT_TIMESTAMP`.The database column contains the comment: 
        /// "When this counter was last updated.".
        /// </summary>
        [Description("When this counter was last updated.")]
        [SyncValue]
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `shop_id`.
        /// The underlying database type is `smallint(5) unsigned`.The database column contains the comment: 
        /// "The shop this counter is for.".
        /// </summary>
        [Description("The shop this counter is for.")]
        [SyncValue]
        public ShopID ShopID
        {
            get { return (ShopID)_shopID; }
            set { _shopID = (UInt16)value; }
        }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        public virtual IWorldStatsCountShopBuyTable DeepCopy()
        {
            return new WorldStatsCountShopBuyTable(this);
        }

        #endregion
    }
}