﻿using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DemoGame.DbObjs;
using NetGore.Db;
using NetGore.Db.QueryBuilder;
using NetGore.World;

namespace DemoGame.Server.Queries
{
    [DbControllerQuery]
    public class DeleteMapQuery : DbQueryNonReader<MapID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMapQuery"/> class.
        /// </summary>
        /// <param name="connectionPool">The connection pool.</param>
        public DeleteMapQuery(DbConnectionPool connectionPool) : base(connectionPool, CreateQuery(connectionPool.QueryBuilder))
        {
        }

        /// <summary>
        /// Creates the query for this class.
        /// </summary>
        /// <param name="qb">The <see cref="IQueryBuilder"/> instance.</param>
        /// <returns>The query for this class.</returns>
        static string CreateQuery(IQueryBuilder qb)
        {
            // DELETE FROM `{0}` WHERE `id`=@id

            var f = qb.Functions;
            var s = qb.Settings;
            var q = qb.Delete(MapTable.TableName).Where(f.Equals(s.EscapeColumn("id"), s.Parameterize("id")));
            return q.ToString();
        }

        /// <summary>
        /// When overridden in the derived class, creates the parameters this class uses for creating database queries.
        /// </summary>
        /// <returns>IEnumerable of all the DbParameters needed for this class to perform database queries. If null,
        /// no parameters will be used.</returns>
        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters("id");
        }

        /// <summary>
        /// When overridden in the derived class, sets the database parameters based on the specified characterID.
        /// </summary>
        /// <param name="p">Collection of database parameters to set the values for.</param>
        /// <param name="map">Item used to execute the query.</param>
        protected override void SetParameters(DbParameterValues p, MapID mapID)
        {
            p["id"] = mapID;
        }
    }
}