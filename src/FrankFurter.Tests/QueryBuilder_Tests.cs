using System;
using Xunit;

namespace FrankFurter.Tests
{
    public class QueryBuilder_Tests
    {
        [Fact]
        public void Ctor_Test()
        {
            var qb = new QueryBuilder("/url");
            var actual = qb.ToString();
            Assert.Equal("/url", actual);
        }

        [Fact]
        public void Ctor_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryBuilder(null));
        }

        [Fact]
        public void Ctor_Empty()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryBuilder(string.Empty));
        }

        [Fact]
        public void Ctor_NoSlashPrefix()
        {
            var qb = new QueryBuilder("test");
            var actual = qb.ToString();
            Assert.Equal("/test", actual);
        }

        [Fact]
        public void Add_WithValue()
        {
            var qb = new QueryBuilder("test")
                .Add("a", "b");
            var actual = qb.ToString();
            Assert.Equal("/test?a=b", actual);
        }

        [Fact]
        public void Add_EncodesValues()
        {
            var qb = new QueryBuilder("test")
                .Add("a", "\"=&");
            var actual = qb.ToString();
            Assert.Equal("/test?a=%22%3d%26", actual);
        }

        [Fact]
        public void Add_IgnoreNull()
        {
            var qb = new QueryBuilder("test")
                .Add("a", null);
            var actual = qb.ToString();
            Assert.Equal("/test", actual);
        }

        [Fact]
        public void Add_IgnoreIfDefault()
        {
            const string defaultValue = "EUR";
            var qb = new QueryBuilder("test")
                .Add("from", "EUR", defaultValue);
            var actual = qb.ToString();
            Assert.Equal("/test", actual);
        }

        [Fact]
        public void Add_IgnoreEmpty()
        {
            var qb = new QueryBuilder("test")
                .Add("a", string.Empty);
            var actual = qb.ToString();
            Assert.Equal("/test", actual);
        }

        [Fact]
        public void Add_IgnoreNullName()
        {
            var qb = new QueryBuilder("test")
                .Add(null, "value");
            var actual = qb.ToString();
            Assert.Equal("/test", actual);
        }

        [Fact]
        public void Add_MultipleValues()
        {
            var qb = new QueryBuilder("test")
                .Add("a", "b")
                .Add("x", "y");
            var actual = qb.ToString();
            Assert.Equal("/test?a=b&x=y", actual);
        }
        
        [Fact]
        public void Add_Decimal()
        {
            var qb = new QueryBuilder("test")
                .Add("a", 1m)       // default - will be excluded
                .Add("b", 2m);
            var actual = qb.ToString();
            Assert.Equal("/test?b=2", actual);
        }



    }
}
