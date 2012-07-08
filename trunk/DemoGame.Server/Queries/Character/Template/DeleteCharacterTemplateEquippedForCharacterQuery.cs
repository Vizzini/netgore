﻿using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DemoGame.Server.DbObjs;
using NetGore.Db;
using NetGore.Db.QueryBuilder;

namespace DemoGame.Server.Queries
{
    [DbControllerQuery]
    public class DeleteCharacterTemplateEquippedForCharacterQuery : DbQueryNonReader<CharacterTemplateID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCharacterTemplateEquippedForCharacterQuery"/> class.
        /// </summary>
        /// <param name="connectionPool">The connection pool.</param>
        public DeleteCharacterTemplateEquippedForCharacterQuery(DbConnectionPool connectionPool)
            : base(connectionPool, CreateQuery(connectionPool.QueryBuilder))
        {
        }

        /// <summary>
        /// Creates the query for this class.
        /// </summary>
        /// <param name="qb">The <see cref="IQueryBuilder"/> instance.</param>
        /// <returns>The query for this class.</returns>
        static string CreateQuery(IQueryBuilder qb)
        {
            /*
                DELETE FROM `{0}`
                    WHERE `character_template_id`=@id
            */

            var f = qb.Functions;
            var s = qb.Settings;
            var q =
                qb.Delete(CharacterTemplateEquippedTable.TableName).Where(f.Equals(s.EscapeColumn("character_template_id"),
                    s.Parameterize("id")));
            return q.ToString();
        }

        /// <summary>
        /// When overridden in the derived class, creates the parameters this class uses for creating database queries.
        /// </summary>
        /// <returns>IEnumerable of all the <see cref="DbParameter"/>s needed for this class to perform database queries.
        /// If null, no parameters will be used.</returns>
        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters("id");
        }

        /// <summary>
        /// When overridden in the derived class, sets the database parameters values <paramref name="p"/>
        /// based on the values specified in the given <paramref name="id"/> parameter.
        /// </summary>
        /// <param name="p">Collection of database parameters to set the values for.</param>
        /// <param name="id">The value or object/struct containing the values used to execute the query.</param>
        protected override void SetParameters(DbParameterValues p, CharacterTemplateID id)
        {
            p["id"] = id;
        }
    }
}