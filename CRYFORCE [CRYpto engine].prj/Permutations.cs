using System;

namespace CRYFORCE.Engine
{
    /// <summary>
    /// Класс для генерирования перестановок элементов множества (n!)
    /// </summary>
    /// <typeparam name="T">Параметр шаблона.</typeparam>
    public class Permutations<T>
    {
        /// <summary>Рабочий материал.</summary>
        private readonly T[] _seed;

        /// <summary>Фиксатор состояния.</summary>
        private int[] _st;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="seed">Массив исходных объектов для генерации перестановок.</param>
        public Permutations(T[] seed)
        {
            _seed = (T[])seed.Clone();
            Reset();
        }

        /// <summary>
        /// Сброс состояния
        /// </summary>
        private void Reset()
        {
            _st = new int[_seed.Length];
            CryforceUtilities.ClearArray(_st);
        }

        /// <summary>
        /// Перестановка двух элементов местами
        /// </summary>
        /// <param name="a">Первый элемент.</param>
        /// <param name="b">Второй элемент.</param>
        private static void Swap(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// Вычисление функции факториала
        /// </summary>
        /// <param name="n">Аргумент.</param>
        /// <returns>Значение n!</returns>
        private static int Fact(int n)
        {
            int result = 1;
            for(int j = 1; j <= n; j++)
            {
                result *= j;
            }

            return result;
        }

        /// <summary>
        /// Получение следующей перестановки элементов
        /// </summary>
        /// <returns>Булевский флаг операции.</returns>
        private bool NextPermutation()
        {
            if(_seed.Length < 2)
            {
                return false;
            }

            int p = _seed.Length - 2;

            while(p >= 0)
            {
                if(_st[p] < _st.Length - 1 - p)
                {
                    Array.Reverse(_seed, (p + 1), (_seed.Length - (p + 1)));
                    Array.Clear(_st, (p + 1), (_seed.Length - (p + 1)));

                    _st[p]++;

                    Swap(ref _seed[p], ref _seed[p + _st[p]]);

                    return true;
                }

                p--;
            }

            return false;
        }

        /// <summary>
        /// Получение набора перестановок
        /// </summary>
        /// <returns>Набор перестановок.</returns>
        /// <remarks>Внимание! Повторный вызов даст перестановки с реверсированного seed, последующий - с прямого и т.д.</remarks>
        public T[][] MakePermutationsSet()
        {
            var result = new T[Fact(_seed.Length)][];

            int i = 0;
            do
            {
                result[i++] = (T[])_seed.Clone();
            } while(NextPermutation());

            Reset();

            return result;
        }
    }
}