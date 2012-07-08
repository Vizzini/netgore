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
using NetGore.Features.Quests;
using NetGore.IO;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Provides a strongly-typed structure for the database table `quest_require_finish_quest`.
    /// </summary>
    public class QuestRequireFinishQuestTable : IQuestRequireFinishQuestTable, IPersistable
    {
        /// <summary>
        /// The number of columns in the database table that this class represents.
        /// </summary>
        public const Int32 ColumnCount = 2;

        /// <summary>
        /// The name of the database table that this class represents.
        /// </summary>
        public const String TableName = "quest_require_finish_quest";

        /// <summary>
        /// Array of the database column names.
        /// </summary>
        static readonly String[] _dbColumns = new string[] { "quest_id", "req_quest_id" };

        /// <summary>
        /// Array of the database column names for columns that are primary keys.
        /// </summary>
        static readonly String[] _dbColumnsKeys = new string[] { "quest_id", "req_quest_id" };

        /// <summary>
        /// Array of the database column names for columns that are not primary keys.
        /// </summary>
        static readonly String[] _dbColumnsNonKey = new string[] { };

        /// <summary>
        /// The field that maps onto the database column `quest_id`.
        /// </summary>
        UInt16 _questID;

        /// <summary>
        /// The field that maps onto the database column `req_quest_id`.
        /// </summary>
        UInt16 _reqQuestID;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestRequireFinishQuestTable"/> class.
        /// </summary>
        public QuestRequireFinishQuestTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestRequireFinishQuestTable"/> class.
        /// </summary>
        /// <param name="questID">The initial value for the corresponding property.</param>
        /// <param name="reqQuestID">The initial value for the corresponding property.</param>
        public QuestRequireFinishQuestTable(QuestID @questID, QuestID @reqQuestID)
        {
            QuestID = @questID;
            ReqQuestID = @reqQuestID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestRequireFinishQuestTable"/> class.
        /// </summary>
        /// <param name="source">IQuestRequireFinishQuestTable to copy the initial values from.</param>
        public QuestRequireFinishQuestTable(IQuestRequireFinishQuestTable source)
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
        public static void CopyValues(IQuestRequireFinishQuestTable source, IDictionary<String, Object> dic)
        {
            dic["quest_id"] = source.QuestID;
            dic["req_quest_id"] = source.ReqQuestID;
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
        /// Copies the values from the given <paramref name="source"/> into this QuestRequireFinishQuestTable.
        /// </summary>
        /// <param name="source">The IQuestRequireFinishQuestTable to copy the values from.</param>
        public void CopyValuesFrom(IQuestRequireFinishQuestTable source)
        {
            QuestID = source.QuestID;
            ReqQuestID = source.ReqQuestID;
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
                case "quest_id":
                    return new ColumnMetadata("quest_id", "The quest that this requirement is for.", "smallint(5) unsigned", null,
                        typeof(UInt16), false, true, false);

                case "req_quest_id":
                    return new ColumnMetadata("req_quest_id",
                        "The quest required to be finished before this quest can be finished.", "smallint(5) unsigned", null,
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
                case "quest_id":
                    return QuestID;

                case "req_quest_id":
                    return ReqQuestID;

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
                case "quest_id":
                    QuestID = (QuestID)value;
                    break;

                case "req_quest_id":
                    ReqQuestID = (QuestID)value;
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

        #region IQuestRequireFinishQuestTable Members

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `quest_id`.
        /// The underlying database type is `smallint(5) unsigned`.The database column contains the comment: 
        /// "The quest that this requirement is for.".
        /// </summary>
        [Description("The quest that this requirement is for.")]
        [SyncValue]
        public QuestID QuestID
        {
            get { return (QuestID)_questID; }
            set { _questID = (UInt16)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `req_quest_id`.
        /// The underlying database type is `smallint(5) unsigned`.The database column contains the comment: 
        /// "The quest required to be finished before this quest can be finished.".
        /// </summary>
        [Description("The quest required to be finished before this quest can be finished.")]
        [SyncValue]
        public QuestID ReqQuestID
        {
            get { return (QuestID)_reqQuestID; }
            set { _reqQuestID = (UInt16)value; }
        }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        public virtual IQuestRequireFinishQuestTable DeepCopy()
        {
            return new QuestRequireFinishQuestTable(this);
        }

        #endregion
    }
}