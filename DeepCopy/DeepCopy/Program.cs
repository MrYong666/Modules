using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace DeepCopy
{
    class Program
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        static void Main(string[] args)
        {
            School school = new School();
            school.schoolName = "一中";
            List<Student> students = new List<Student>();
            Student student = new Student();
            student.Age = 1;
            student.name = "zhangsan";
            students.Add(student);
            school.Student = students;

            var newObj = school.DeepCopy();

            newObj.schoolName = "更改后";
            newObj.Student[0].name = "Student Update1";
            newObj.Student[0].Age = 22;
            newObj.Student[1].name = "Student Update2";
            newObj.Student[0].Age = 33;






        }
        [StructLayout(LayoutKind.Sequential)]
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Sex { get; set; }
        }
        /// <summary>
        /// 深度克隆实现原理
        /// </summary>
        /// <param name="school"></param>
        public static void DeepDemo(School school)
        {
            //School里面有引用类型，所以和克隆的对象享有一个引用；
            var schoolCopy = (School)CloneMethod.Invoke(school, null);
            schoolCopy.Student[0].name = "浅克隆引用类型";

            // Student里面没有引用类型，相当于直接克隆了一个新对象
            var stuObj = (Student)CloneMethod.Invoke(school.Student[0], null);
            stuObj.Age = school.Student[0].Age + 1111;

        }
        public static void SetValue(School school)
        {
            var stuObj = (Student)CloneMethod.Invoke(school.Student[0], null);
            stuObj.Age = school.Student[0].Age + 1111;
            var stuObj1 = (Student)CloneMethod.Invoke(school.Student[1], null);

            var stuObj2 = (School)CloneMethod.Invoke(school, null);
            stuObj2.Student[0] = stuObj;
            stuObj2.Student[0].Age = 11111111;
            Dictionary<object, object> t = new Dictionary<object, object>();
            t.Add(1, stuObj);
            t.Add(2, stuObj1);

            var type = typeof(School);
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            FieldInfo[] fieldInfos = school.GetType().GetFields(bindingFlags);
            foreach (var item in fieldInfos)
            {
                var name = item.Name.Contains("schoolName");
                if (item.Name.Contains("schoolName"))
                {
                    item.SetValue(school, "ttt");
                    continue;
                }
            }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public class School
    {
        public string schoolName { get; set; }
        public List<Student> Student { get; set; }
    }
    [StructLayout(LayoutKind.Sequential)]
    public class Student
    {
        public int Age { get; set; }
        public string name { get; set; }
    }



}
