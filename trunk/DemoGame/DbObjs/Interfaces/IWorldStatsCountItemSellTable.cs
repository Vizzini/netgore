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
using System.Linq;

namespace DemoGame.DbObjs
{
    /// <summary>
    /// Interface for a class that can be used to serialize values to the database table `world_stats_count_item_sell`.
    /// </summary>
    public interface IWorldStatsCountItemSellTable
    {
        /// <summary>
        /// Gets the value of the database column `count`.
        /// </summary>
        Int32 Count { get; }

        /// <summary>
        /// Gets the value of the database column `item_template_id`.
        /// </summary>
        ItemTemplateID ItemTemplateID { get; }

        /// <summary>
        /// Gets the value of the database column `last_update`.
        /// </summary>
        DateTime LastUpdate { get; }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        IWorldStatsCountItemSellTable DeepCopy();
    }
}