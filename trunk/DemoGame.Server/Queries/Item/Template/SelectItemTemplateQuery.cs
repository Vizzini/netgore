using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DemoGame.DbObjs;
using DemoGame.Server.DbObjs;
using NetGore.Db;
using NetGore.Db.QueryBuilder;

namespace DemoGame.Server.Queries
{
    [DbControllerQuery]
    public class SelectItemTemplateQuery : DbQueryReader<ItemTemplateID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectItemTemplateQuery"/> class.
        /// </summary>
        /// <param name="connectionPool">The connection pool.</param>
        public SelectItemTemplateQuery(DbConnectionPool connectionPool)
            : base(connectionPool, CreateQuery(connectionPool.QueryBuilder))
        {
            QueryAsserts.ArePrimaryKeys(ItemTemplateTable.DbKeyColumns, "id");
        }

        /// <summary>
        /// Creates the query for this class.
        /// </summary>
        /// <param name="qb">The <see cref="IQueryBuilder"/> instance.</param>
        /// <returns>The query for this class.</returns>
        static string CreateQuery(IQueryBuilder qb)
        {
            // SELECT * FROM `{0}` WHERE `id`=@id

            var f = qb.Functions;
            var s = qb.Settings;
            var q = qb.Select(ItemTemplateTable.TableName).AllColumns().Where(f.Equals(s.EscapeColumn("id"), s.Parameterize("id")));
            return q.ToString();
        }

        public IItemTemplateTable Execute(ItemTemplateID id)
        {
            var v = new ItemTemplateTable();

            using (var r = ExecuteReader(id))
            {
                if (!r.Read())
                    return null;

                v.ReadValues(r);
            }

            return v;
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
        /// based on the values specified in the given <paramref name="item"/> parameter.
        /// </summary>
        /// <param name="p">Collection of database parameters to set the values for.</param>
        /// <param name="item">The value or object/struct containing the values used to execute the query.</param>
        protected override void SetParameters(DbParameterValues p, ItemTemplateID item)
        {
            p["id"] = item;
        }
    }
}