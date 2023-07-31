
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
        /// ���������� ������ ������ �� ���������� �����������
        /// </summary>
        /// <param name="type">������������ �����������</param>
        /// <returns>������-����� ��� null</returns>
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
        /// ���������������� ������ ������
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
    /// ��� ������ ������
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// ������ ������ � ����� �� ������
        /// </summary>
        TopLeft,
        /// <summary>
        /// ������ ������ � ������ �� ������
        /// </summary>
        TopRight,
        /// <summary>
        /// ������ ����� � ����� �� ������
        /// </summary>
        BottomLeft,
        /// <summary>
        /// ������ ����� � ������ �� ������
        /// </summary>
        BottomRight
    }
}