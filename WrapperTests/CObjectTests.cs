using Wrapper;
using FluentAssertions;

namespace WrapperTests
{
    public class CObjectTests 
    {
        unsafe class RefCounter
        {
            public int Counter { get => *counter; }

            internal RefCounter(int* counter)
            {
                this.counter = counter;
                System.Threading.Interlocked.Increment(ref *counter);
            }

            ~RefCounter()
            {
                System.Threading.Interlocked.Decrement(ref *counter);
            }

            int* counter;
        }


        [Fact]
        public void ItCreatesACObject()
        {
            var cObj = new CObject("init", null);

            cObj.Inner.Should().BeNull();
            cObj.Name.Should().Be("init");
            cObj.Dispose();
        }

        [Fact]
        public void ItSetsInnerObject()
        {
            var cObj = new CObject("init", null);

            cObj.Inner = new int[] { 1, 2, 3 };

            var castInner2 = cObj.Inner as int[];
            castInner2.Should().NotBeNull();
            castInner2.Should().HaveCount(3);
            castInner2.Should().BeOfType<int[]>();

            cObj.Dispose();
        }


        [Fact]
        unsafe public void ItDisposesInnerObject()
        {
            var refs = 0;
            
            RunDisposeTest(&refs);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            refs.Should().Be(0);

            unsafe void RunDisposeTest(int* counter)
            {
                var rc = new RefCounter(counter);
                var cObj = new CObject("refcount");

                cObj.Inner = rc;
                var castInner = rc; 

                castInner.Should().NotBeNull();
                castInner.Should().BeOfType<RefCounter>();
                castInner!.Counter.Should().Be(1);

                cObj.Inner = "changed";

                var castInner2 = cObj.Inner as string;
                castInner2.Should().NotBeNull();
                castInner2.Should().Be("changed");

                cObj.Dispose();
            }
        }
    }
}