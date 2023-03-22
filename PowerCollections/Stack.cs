using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PowerCollections
{
    public class Stack<T> : IEnumerable<T>
    {
        private T[] array; // элементы стека
        private int count;  // количество элементов
        private int capacity;
        // количество элементов в массиве по умолчанию

        public IEnumerator<T> GetEnumerator()
        
            for (int i = array.Length - 1; i > -1; i--)
            {
                yield return array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public Stack(int length)
        {
            capacity = length;
            if (capacity <= 0)
            {
                throw new ArgumentException("Максимальное количество элементов стека отрицательное или равно нулю");
            }
            array = new T[capacity];
            
        }
        public Stack()
        {
            capacity = 100;
            if (capacity <= 0)
            {
                throw new ArgumentException("Максимальное количество элементов стека отрицательное или равно нулю");
            }
            array = new T[capacity];

        }

        public int Count //счетчик количества элементов
        {
            get { return count; }
        }

        public int Capacity //количество элементов
        {

            get { return capacity; }


        }

        
        // пуст ли стек
        public bool IsEmpty //пуст ли стек
        {
            get { return count == 0; }
        }
        
       
        // добвление элемента
        public void Push(T element)
        {
            // если стек заполнен
            if (count == array.Length)
                throw new InvalidOperationException("Переполнение стека");
            array[count++] = element;
        }
        // извлечение элемента
        public T Pop()
        {
            
            if (IsEmpty)
                throw new InvalidOperationException("Стек пуст");
            T element = array[--count];
            array[count] = default(T); // сбрасываем ссылку
            return element;
        }
        // возвращаем элемент из верхушки стека
        public T Top()
        {
            
            if (IsEmpty)
                throw new InvalidOperationException("Стек пуст");
            //return items[count - 1];
            T element = array[count - 1];
            return element;
        }

    }
}
