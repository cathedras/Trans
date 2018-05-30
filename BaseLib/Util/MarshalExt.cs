using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace myzy.Util
{
    public class MarshalExt
    {
        public struct user_group_t
        {
            public int id;
            public string name;
        }


        public struct user_group_list
        {
            public int group_array_count;

            public IntPtr group_array; //指向 user_group_t类型的指针
        }

        public static List<T> MarshalPtrToStructArray<T>(IntPtr p, int count)
        {
            List<T> l = new List<T>();
            for (int i = 0; i < count; i++, p = new IntPtr(p.ToInt32() + Marshal.SizeOf(typeof(T))))
            {
                T t = (T) Marshal.PtrToStructure(p, typeof(T));
                l.Add(t);
            }
            return l;
        }

        /*
         使用Marshal.PtrToStructure把指向结构体的指针转换为具体的结构体

    user_group_list   tructList = (user_group_list)Marshal.PtrToStructure(ptrStructGroupList, typeof(user_group_list));

    再使用泛型转换函数实现转换

    List<user_group_t> listGroupTemp = MarshalPtrToStructArray<user_group_t>(structList.group_array, structList.group_array_count);
         */
    }
}