
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine;
using System;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        

        private Dictionary<NeighborType, CellComponent> _neighbors;


        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type)
        {
            if (_neighbors.ContainsKey(type) && _neighbors.ContainsValue(_neighbors[type]))
                return _neighbors[type];
            else return null;
        }


        public List<CellComponent> GetAviableMoves()
        {
            List <CellComponent> result = new List<CellComponent>();
            foreach (var direct in GetEnumValues<NeighborType>())
            {
                if (this.GetNeighbors(direct) != null && this.GetNeighbors(direct).Pair == null)
                {
                    if (GameManager.Self.Turn == ColorType.White)
                    {
                        if (direct == NeighborType.BottomLeft || direct == NeighborType.BottomRight)
                            result.Add(this.GetNeighbors(direct));
                    }
                    if (GameManager.Self.Turn == ColorType.Black)
                    {
                        if (direct == NeighborType.TopLeft || direct == NeighborType.TopRight)
                        {
                            result.Add(this.GetNeighbors(direct));
                        }
                    }
                }
            }
            return result;
        }

        public List<CellComponent> GetAviableDown()
        {
            List<CellComponent> result = new List<CellComponent>();

            foreach (var direct in GetEnumValues<NeighborType>())
            {
                if (this.GetNeighbors(direct) != null && this.GetNeighbors(direct).Pair != null)
                    if (this.GetNeighbors(direct).Pair.Color != this.Pair.Color)
                    { 
                        if (this.GetNeighbors(direct).GetNeighbors(direct) != null && this.GetNeighbors(direct).GetNeighbors(direct).Pair == null)
                            if (GameManager.Self.Turn == ColorType.White)
                            {
                                if (direct == NeighborType.BottomLeft || direct == NeighborType.BottomRight)
                                    result.Add(this.GetNeighbors(direct).GetNeighbors(direct));
                            }
                            if (GameManager.Self.Turn == ColorType.Black)
                            {
                                if (direct == NeighborType.TopLeft || direct == NeighborType.TopRight)
                                    result.Add(this.GetNeighbors(direct).GetNeighbors(direct));
                            }
                    }
            }
            return result;
        }


        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }

        /// <summary>
        /// Конфигурирование связей клеток
        /// </summary>
		public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
        {
            if (_neighbors != null) return;
            _neighbors = neighbors;
        }
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight
    }
}