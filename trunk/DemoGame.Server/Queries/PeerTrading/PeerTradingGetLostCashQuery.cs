﻿using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DemoGame.Server.DbObjs;
using NetGore.Db;
using NetGore.Db.QueryBuilder;

namespace DemoGame.Server.Queries
{
    [DbControllerQuery]
    public class PeerTradingGetLostCashQuery : DbQueryReader<CharacterID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeerTradingGetLostCashQuery"/> class.
        /// </summary>
        /// <param name="connectionPool">The <see cref="DbConnectionPool"/> to use for creating connections to execute the query on.</param>
        public PeerTradingGetLostCashQuery(DbConnectionPool connectionPool)
            : base(connectionPool, CreateQuery(connectionPool.QueryBuilder))
        {
            QueryAsserts.ContainsColumns(ActiveTradeCashTable.DbColumns, "cash");
            QueryAsserts.ArePrimaryKeys(ActiveTradeCashTable.DbKeyColumns, "character_id");
        }

        /// <summary>
        /// Creates the query for this class.
        /// </summary>
        /// <param name="qb">The <see cref="IQueryBuilder"/> instance.</param>
        /// <returns>The query for this class.</returns>
        static string CreateQuery(IQueryBuilder qb)
        {
            // SELECT `cash` FROM `{0}` WHERE `character_id` = @characterID

            var f = qb.Functions;
            var s = qb.Settings;
            var q =
                qb.Select(ActiveTradeCashTable.TableName).Add("cash").Where(f.Equals(s.EscapeColumn("character_id"),
                    s.Parameterize("characterID")));
            return q.ToString();
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="characterID">The ID of the character to get the lost cash for.</param>
        /// <param name="cash">When this method returns true, contains the amount of lost cash for the <paramref name="characterID"/>.</param>
        /// <returns>True if a value existed in the table for the <paramref name="characterID"/>; otherwise false.</returns>
        public bool TryExecute(CharacterID characterID, out int cash)
        {
            cash = 0;

            using (var r = ExecuteReader(characterID))
            {
                if (!r.Read())
                    return false;

                cash = r.GetInt32("cash");
            }

            return true;
        }

        #region Overrides of DbQueryBase

        /// <summary>
        /// When overridden in the derived class, creates the parameters this class uses for creating database queries.
        /// </summary>
        /// <returns>The <see cref="DbParameter"/>s needed for this class to perform database queries.
        /// If null, no parameters will be used.</returns>
        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters("characterID");
        }

        #endregion

        #region Overrides of DbQueryReader<CharacterID>

        /// <summary>
        /// When overridden in the derived class, sets the database parameters values <paramref name="p"/>
        /// based on the values specified in the given <paramref name="item"/> parameter.
        /// </summary>
        /// <param name="p">Collection of database parameters to set the values for.</param>
        /// <param name="item">The value or object/struct containing the values used to execute the query.</param>
        protected override void SetParameters(DbParameterValues p, CharacterID item)
        {
            p["characterID"] = (int)item;
        }

        #endregion
    }
}