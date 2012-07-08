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
using NetGore.IO;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Provides a strongly-typed structure for the database table `alliance_hostile`.
    /// </summary>
    public class AllianceHostileTable : IAllianceHostileTable, IPersistable
    {
        /// <summary>
        /// The number of columns in the database table that this class represents.
        /// </summary>
        public const Int32 ColumnCount = 2;

        /// <summary>
        /// The name of the database table that this class represents.
        /// </summary>
        public const String TableName = "alliance_hostile";

        /// <summary>
        /// Array of the database column names.
        /// </summary>
        static readonly String[] _dbColumns = new string[] { "alliance_id", "hostile_id" };

        /// <summary>
        /// Array of the database column names for columns that are primary keys.
        /// </summary>
        static readonly String[] _dbColumnsKeys = new string[] { "alliance_id", "hostile_id" };

        /// <summary>
        /// Array of the database column names for columns that are not primary keys.
        /// </summary>
        static readonly String[] _dbColumnsNonKey = new string[] { };

        /// <summary>
        /// The field that maps onto the database column `alliance_id`.
        /// </summary>
        Byte _allianceID;

        /// <summary>
        /// The field that maps onto the database column `hostile_id`.
        /// </summary>
        Byte _hostileID;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllianceHostileTable"/> class.
        /// </summary>
        public AllianceHostileTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllianceHostileTable"/> class.
        /// </summary>
        /// <param name="allianceID">The initial value for the corresponding property.</param>
        /// <param name="hostileID">The initial value for the corresponding property.</param>
        public AllianceHostileTable(AllianceID @allianceID, AllianceID @hostileID)
        {
            AllianceID = @allianceID;
            HostileID = @hostileID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllianceHostileTable"/> class.
        /// </summary>
        /// <param name="source">IAllianceHostileTable to copy the initial values from.</param>
        public AllianceHostileTable(IAllianceHostileTable source)
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
        public static void CopyValues(IAllianceHostileTable source, IDictionary<String, Object> dic)
        {
            dic["alliance_id"] = source.AllianceID;
            dic["hostile_id"] = source.HostileID;
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
        /// Copies the values from the given <paramref name="source"/> into this AllianceHostileTable.
        /// </summary>
        /// <param name="source">The IAllianceHostileTable to copy the values from.</param>
        public void CopyValuesFrom(IAllianceHostileTable source)
        {
            AllianceID = source.AllianceID;
            HostileID = source.HostileID;
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
                case "alliance_id":
                    return new ColumnMetadata("alliance_id", "The alliance that is hotile.", "tinyint(3) unsigned", null,
                        typeof(Byte), false, true, false);

                case "hostile_id":
                    return new ColumnMetadata("hostile_id",
                        "The alliance that this alliance (alliance_id) is hostile towards by default.", "tinyint(3) unsigned",
                        null, typeof(Byte), false, true, false);

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
                case "alliance_id":
                    return AllianceID;

                case "hostile_id":
                    return HostileID;

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
                case "alliance_id":
                    AllianceID = (AllianceID)value;
                    break;

                case "hostile_id":
                    HostileID = (AllianceID)value;
                    break;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        #region IAllianceHostileTable Members

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `alliance_id`.
        /// The underlying database type is `tinyint(3) unsigned`.The database column contains the comment: 
        /// "The alliance that is hotile.".
        /// </summary>
        [Description("The alliance that is hotile.")]
        [SyncValue]
        public AllianceID AllianceID
        {
            get { return (AllianceID)_allianceID; }
            set { _allianceID = (Byte)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `hostile_id`.
        /// The underlying database type is `tinyint(3) unsigned`.The database column contains the comment: 
        /// "The alliance that this alliance (alliance_id) is hostile towards by default.".
        /// </summary>
        [Description("The alliance that this alliance (alliance_id) is hostile towards by default.")]
        [SyncValue]
        public AllianceID HostileID
        {
            get { return (AllianceID)_hostileID; }
            set { _hostileID = (Byte)value; }
        }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        public virtual IAllianceHostileTable DeepCopy()
        {
            return new AllianceHostileTable(this);
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