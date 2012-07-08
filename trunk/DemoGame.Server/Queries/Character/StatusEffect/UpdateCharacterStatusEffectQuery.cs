﻿using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DemoGame.DbObjs;
using DemoGame.Server.DbObjs;
using NetGore.Db;
using NetGore.Db.QueryBuilder;

namespace DemoGame.Server.Queries
{
    [DbControllerQuery]
    public class UpdateCharacterStatusEffectQuery : DbQueryNonReader<ICharacterStatusEffectTable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCharacterStatusEffectQuery"/> class.
        /// </summary>
        /// <param name="connectionPool">The connection pool.</param>
        public UpdateCharacterStatusEffectQuery(DbConnectionPool connectionPool)
            : base(connectionPool, CreateQuery(connectionPool.QueryBuilder))
        {
            QueryAsserts.ArePrimaryKeys(CharacterStatusEffectTable.DbKeyColumns, "id");
        }

        /// <summary>
        /// Creates the query for this class.
        /// </summary>
        /// <param name="qb">The <see cref="IQueryBuilder"/> instance.</param>
        /// <returns>The query for this class.</returns>
        static string CreateQuery(IQueryBuilder qb)
        {
            /*
                UPDATE `{0}` SET {1}
                    WHERE `id` = @id
            */

            var s = qb.Settings;
            var f = qb.Functions;
            var q =
                qb.Update(CharacterStatusEffectTable.TableName).AddAutoParam(CharacterStatusEffectTable.DbNonKeyColumns).Where(
                    f.Equals(s.EscapeColumn("id"), s.Parameterize("id")));
            return q.ToString();
        }

        /// <summary>
        /// When overridden in the derived class, creates the parameters this class uses for creating database queries.
        /// </summary>
        /// <returns>IEnumerable of all the DbParameters needed for this class to perform database queries. If null,
        /// no parameters will be used.</returns>
        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters(CharacterStatusEffectTable.DbColumns);
        }

        /// <summary>
        /// When overridden in the derived class, sets the database parameters based on the specified characterID.
        /// </summary>
        /// <param name="p">Collection of database parameters to set the values for.</param>
        /// <param name="item">Item used to execute the query.</param>
        protected override void SetParameters(DbParameterValues p, ICharacterStatusEffectTable item)
        {
            item.CopyValues(p);
        }
    }
}