﻿//-----------------------------------------------------------------------
// <copyright file="RedisSnapshotStoreSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using Akka.Configuration;
using Akka.Persistence.TestKit.Snapshot;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Persistence.Redis.Tests
{
    [Collection("RedisSpec")]
    public class RedisSnapshotStoreSpec : SnapshotStoreSpec
    {
        private static readonly Config SpecConfig;
        private static readonly string KeyPrefix;

        static RedisSnapshotStoreSpec()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["redis"].ConnectionString;
            var database = ConfigurationManager.AppSettings["redisDatabase"];

            SpecConfig = ConfigurationFactory.ParseString(@"
                akka.test.single-expect-default = 3s
                akka.persistence {
                    publish-plugin-commands = on
                    snapshot-store {
                        plugin = ""akka.persistence.snapshot-store.redis""
                        redis {
                            class = ""Akka.Persistence.Redis.Snapshot.RedisSnapshotStore, Akka.Persistence.Redis""
                            configuration-string = """ + connectionString + @"""
                            plugin-dispatcher = ""akka.actor.default-dispatcher""
                            ttl = 1h
                            database = """ + database + @"""
                            key-prefix = ""akka:persistence:snapshots""
                        }
                    }
                }");

            KeyPrefix = SpecConfig.GetString("akka.persistence.snapshot-store.redis.key-prefix");
        }

        public RedisSnapshotStoreSpec(ITestOutputHelper output)
            : base(SpecConfig, typeof(RedisSnapshotStoreSpec).Name, output)
        {
            RedisPersistence.Get(Sys);
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DbUtils.Clean(KeyPrefix);
        }
    }
}