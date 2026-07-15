using System;
using System.Collections.Generic;
using System.Reflection;
using Android.Util;

namespace SFKBle_Admin
{
    public class ProbeTable
    {
        private readonly string TAG = typeof(ProbeTable).Name;

        Dictionary<Tuple<int, int>, Type> mProbeTable = new Dictionary<Tuple<int, int>, Type>();
        public ProbeTable AddProduct(int vendorId, int productId,
                Type driverClass)
        {
            var key = new Tuple<int, int>(vendorId, productId);

            if (!mProbeTable.ContainsKey(key))
                mProbeTable.Add(key, driverClass);

            return this;
        }

        public ProbeTable AddDriver(Type driverClass)
        {
            MethodInfo m = driverClass.GetMethod("GetSupportedDevices");

            var devices =  (Dictionary<int, int[]>)m.Invoke(null, null);

            foreach (var vendorId in devices.Keys)
            {
                var productIds = devices[vendorId];

                foreach (var productId in productIds)
                {
                    try
                    {
                        AddProduct(vendorId, productId, driverClass);
                    }
                    catch (Exception)
                    {   
                        throw;
                    }
                }
            }

            return this;

        }

        public Type FindDriver(int vendorId, int productId)
        {
            var pair = new Tuple<int, int>(vendorId, productId);

            return mProbeTable.ContainsKey(pair) ? mProbeTable[pair] : null;
        }

    }
}