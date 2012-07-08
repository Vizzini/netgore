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
using NetGore.IO;
using NetGore.World;

namespace DemoGame.DbObjs
{
    /// <summary>
    /// Provides a strongly-typed structure for the database table `map`.
    /// </summary>
    public class MapTable : IMapTable, IPersistable
    {
        /// <summary>
        /// The number of columns in the database table that this class represents.
        /// </summary>
        public const Int32 ColumnCount = 2;

        /// <summary>
        /// The name of the database table that this class represents.
        /// </summary>
        public const String TableName = "map";

        /// <summary>
        /// Array of the database column names.
        /// </summary>
        static readonly String[] _dbColumns = new string[] { "id", "name" };

        /// <summary>
        /// Array of the database column names for columns that are primary keys.
        /// </summary>
        static readonly String[] _dbColumnsKeys = new string[] { "id" };

        /// <summary>
        /// Array of the database column names for columns that are not primary keys.
        /// </summary>
        static readonly String[] _dbColumnsNonKey = new string[] { "name" };

        /// <summary>
        /// The field that maps onto the database column `id`.
        /// </summary>
        UInt16 _iD;

        /// <summary>
        /// The field that maps onto the database column `name`.
        /// </summary>
        String _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTable"/> class.
        /// </summary>
        public MapTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTable"/> class.
        /// </summary>
        /// <param name="iD">The initial value for the corresponding property.</param>
        /// <param name="name">The initial value for the corresponding property.</param>
        public MapTable(MapID @iD, String @name)
        {
            ID = @iD;
            Name = @name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTable"/> class.
        /// </summary>
        /// <param name="source">IMapTable to copy the initial values from.</param>
        public MapTable(IMapTable source)
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
        public static void CopyValues(IMapTable source, IDictionary<String, Object> dic)
        {
            dic["id"] = source.ID;
            dic["name"] = source.Name;
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
        /// Copies the values from the given <paramref name="source"/> into this MapTable.
        /// </summary>
        /// <param name="source">The IMapTable to copy the values from.</param>
        public void CopyValuesFrom(IMapTable source)
        {
            ID = source.ID;
            Name = source.Name;
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
                case "id":
                    return new ColumnMetadata("id", "The unique ID of the map.", "smallint(5) unsigned", null, typeof(UInt16),
                        false, true, false);

                case "name":
                    return new ColumnMetadata("name", "Name of the map.", "varchar(255)", null, typeof(String), false, false,
                        false);

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
                case "id":
                    return ID;

                case "name":
                    return Name;

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
                case "id":
                    ID = (MapID)value;
                    break;

                case "name":
                    Name = (String)value;
                    break;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        #region IMapTable Members

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `id`.
        /// The underlying database type is `smallint(5) unsigned`.The database column contains the comment: 
        /// "The unique ID of the map.".
        /// </summary>
        [Description("The unique ID of the map.")]
        [SyncValue]
        public MapID ID
        {
            get { return (MapID)_iD; }
            set { _iD = (UInt16)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `name`.
        /// The underlying database type is `varchar(255)`.The database column contains the comment: 
        /// "Name of the map.".
        /// </summary>
        [Description("Name of the map.")]
        [SyncValue]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        public virtual IMapTable DeepCopy()
        {
            return new MapTable(this);
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