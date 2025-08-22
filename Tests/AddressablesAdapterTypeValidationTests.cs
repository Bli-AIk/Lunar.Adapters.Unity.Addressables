using System;
using System.Threading;
using NUnit.Framework;

namespace Lunar.Adapters.Unity.Addressables.Tests
{
    public class AddressablesAdapterTypeValidationTests
    {
        private AddressablesAdapter _addrAdapter;

        [SetUp]
        public void SetUp()
        {
            _addrAdapter = new AddressablesAdapter();
        }

        [Test]
        public void LoadAsync_ThrowsImmediately_ForNonUnityType()
        {
            // Should throw before any Addressables call because of type validation
            Assert.Throws<InvalidOperationException>(() =>
            {
                // We just call the method; it should synchronously throw
                var _ = _addrAdapter.LoadAsync<int>("someKey");
            });
        }

        [Test]
        public void LoadAllAsync_StringPath_ThrowsImmediately_ForNonUnityType()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                // static overload also validates type right away
                var task = AddressablesAdapter.LoadAllAsync<int>("somePath", null, UnityEngine.AddressableAssets.Addressables.MergeMode.None,
                    CancellationToken.None);
            });
        }

        [Test]
        [Obsolete("Obsolete")]
        public void Obsolete_LoadAll_Static_ThrowsImmediately_ForNonUnityType()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = AddressablesAdapter.LoadAll<int>("somePath", null);
            });
        }
    }
}