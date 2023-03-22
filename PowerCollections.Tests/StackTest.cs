using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerCollections.Tests
{
    [TestClass]
    public class StackTest
    {
        [TestMethod]

         public void Constructor_should_check_the_value_of_capacity()  //проверить значение максимального количество элементов
         {
            Stack<int> ST = new Stack<int>(5);
            Assert.AreEqual(5, ST.Capacity);
         }

        [TestMethod]
        public void Constructor_should_create_empty_stack_with_zero_count() 
        {
            Stack<int> ST = new Stack<int>(5);
            
            Assert.AreEqual(true, ST.IsEmpty);
            Assert.AreEqual(0, ST.Count);
        }

        [TestMethod]
        public void Constructor_IsEmty_return_false_if_Stack_is_not_empty_and_count_is_not_0() //проверить не пустой стек
        {
            Stack<int> ST = new Stack<int>(5);
            ST.Push(1);
            Assert.AreEqual(false, ST.IsEmpty);
            Assert.AreEqual(1, ST.Count);
        }

        [TestMethod]
        public void Push_stack_and_return_top_element() //проверить метод топ, правильно ли выводит последний элемент
        {
            Stack<int> ST = new Stack<int>(5);
            for (int i = 0; i < 5; i++)
            {
                ST.Push(i);
            }
            
            Assert.AreEqual(4, ST.Top());

        }

        [TestMethod]
        public void Push_stack_return_top_element_and_check_delete_top_element_from_stack() //проверить метод поп, вывод последнего элемента и его удаление, соответственно количество элементов уменьшится на 1
        {
            Stack<int> ST = new Stack<int>(5);
            for (int i = 1; i < 5; i++)
            {
                ST.Push(i);
            }

            Assert.AreEqual(4, ST.Pop()); //высший элемент
            Assert.AreEqual(3, ST.Count); //количество элементов, уменьшенное на 1, тк высший удален

        }

        [TestMethod]
        public void IEnumerator_should_return_elements_of_stack_in_revers() // проверить, работает ли ienumerator, то есть запись элементов стека в обратном порядке
        {
            var array = new int[] { 4, 3, 2, 1, 0 };

            Stack<int> ST = new Stack<int>(5);
            for (int i = 0; i < 5; i++)
            {
                ST.Push(i);
            }


            CollectionAssert.AreEqual(array, ST.ToArray());

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_shold_throw_exception_if_request_negatve_stack_capacity() //когда макс количество элементов выбрано меньше нуля
        {
            Stack<int> ST = new Stack<int>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Calling_Push_on_a_full_stack_throws_an_exception() //когда заполняем массив большим количеством данных, чем capacity
        {
            Stack<int> ST = new Stack<int>(5);
            for (int i = 0; i < 6; i++)
            {
                ST.Push(i);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Check_result_of_deleting_top_element_in_empty_stack() //удаление элемента из пустого стека
        {
            Stack<int> ST = new Stack<int>(4);
            ST.Pop();

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Check_top_element_in_empty_stack() //последний элемень пустого стека
        {
            Stack<int> ST = new Stack<int>(8);
            ST.Top();

        }

    }
}




