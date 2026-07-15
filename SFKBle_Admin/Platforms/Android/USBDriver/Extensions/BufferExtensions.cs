using System;
using Android.Runtime;
using Java.Nio;

namespace SFKBle_Admin
{
    public static class BufferExtensions
    {
        static IntPtr _byteBufferClassRef;
        static IntPtr _byteBufferGetBii;

        public static byte[] ToByteArray(this ByteBuffer buffer)
        {
            IntPtr classHandle = JNIEnv.FindClass("java/nio/ByteBuffer");
            IntPtr methodId = JNIEnv.GetMethodID(classHandle, "array", "()[B");
            IntPtr resultHandle = JNIEnv.CallObjectMethod(buffer.Handle, methodId);

            byte[] result = JNIEnv.GetArray<byte>(resultHandle);

            JNIEnv.DeleteLocalRef(resultHandle);

            return result;
        }
    }
}