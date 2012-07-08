﻿using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DemoGame.Server.DbObjs;
using NetGore.Db;
using NetGore.Db.QueryBuilder;

namespace DemoGame.Server.Queries
{
    [DbControllerQuery]
    public class InsertCharacterTemplateSkillQuery : DbQueryNonReader<KeyValuePair<CharacterTemplateID, SkillType>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCharacterTemplateSkillQuery"/> class.
        /// </summary>
        /// <param name="connectionPool">The connection pool.</param>
        public InsertCharacterTemplateSkillQuery(DbConnectionPool connectionPool)
            : base(connectionPool, CreateQuery(connectionPool.QueryBuilder))
        {
            QueryAsserts.AreColumns(CharacterTemplateSkillTable.DbColumns, "character_template_id", "skill_id");
        }

        /// <summary>
        /// Creates the query for this class.
        /// </summary>
        /// <param name="qb">The <see cref="IQueryBuilder"/> instance.</param>
        /// <returns>The query for this class.</returns>
        static string CreateQuery(IQueryBuilder qb)
        {
            /*
                INSERT IGNORE INTO `{0}` {1}`
            */

            var q =
                qb.Insert(CharacterTemplateSkillTable.TableName).IgnoreExists().AddAutoParam(CharacterTemplateSkillTable.DbColumns);
            return q.ToString();
        }

        public void Execute(CharacterTemplateID charTemplateID, SkillType skill)
        {
            Execute(new KeyValuePair<CharacterTemplateID, SkillType>(charTemplateID, skill));
        }

        /// <summary>
        /// When overridden in the derived class, creates the parameters this class uses for creating database queries.
        /// </summary>
        /// <returns>IEnumerable of all the <see cref="DbParameter"/>s needed for this class to perform database queries.
        /// If null, no parameters will be used.</returns>
        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters(CharacterTemplateSkillTable.DbColumns);
        }

        /// <summary>
        /// When overridden in the derived class, sets the database parameters values <paramref name="p"/>
        /// based on the values specified in the given <paramref name="item"/> parameter.
        /// </summary>
        /// <param name="p">Collection of database parameters to set the values for.</param>
        /// <param name="item">The value or object/struct containing the values used to execute the query.</param>
        protected override void SetParameters(DbParameterValues p, KeyValuePair<CharacterTemplateID, SkillType> item)
        {
            p["character_template_id"] = (int)item.Key;
            p["skill_id"] = (int)item.Value;
        }
    }
}