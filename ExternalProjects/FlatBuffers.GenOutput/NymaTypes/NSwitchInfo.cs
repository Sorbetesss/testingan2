// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace NymaTypes
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct NSwitchInfo : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_22_9_24(); }
  public static NSwitchInfo GetRootAsNSwitchInfo(ByteBuffer _bb) { return GetRootAsNSwitchInfo(_bb, new NSwitchInfo()); }
  public static NSwitchInfo GetRootAsNSwitchInfo(ByteBuffer _bb, NSwitchInfo obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public NSwitchInfo __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public uint DefaultPosition { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
  public NymaTypes.NSwitchPosition? Positions(int j) { int o = __p.__offset(6); return o != 0 ? (NymaTypes.NSwitchPosition?)(new NymaTypes.NSwitchPosition()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int PositionsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<NymaTypes.NSwitchInfo> CreateNSwitchInfo(FlatBufferBuilder builder,
      uint DefaultPosition = 0,
      VectorOffset PositionsOffset = default(VectorOffset)) {
    builder.StartTable(2);
    NSwitchInfo.AddPositions(builder, PositionsOffset);
    NSwitchInfo.AddDefaultPosition(builder, DefaultPosition);
    return NSwitchInfo.EndNSwitchInfo(builder);
  }

  public static void StartNSwitchInfo(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddDefaultPosition(FlatBufferBuilder builder, uint DefaultPosition) { builder.AddUint(0, DefaultPosition, 0); }
  public static void AddPositions(FlatBufferBuilder builder, VectorOffset PositionsOffset) { builder.AddOffset(1, PositionsOffset.Value, 0); }
  public static VectorOffset CreatePositionsVector(FlatBufferBuilder builder, Offset<NymaTypes.NSwitchPosition>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreatePositionsVectorBlock(FlatBufferBuilder builder, Offset<NymaTypes.NSwitchPosition>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreatePositionsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<NymaTypes.NSwitchPosition>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreatePositionsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<NymaTypes.NSwitchPosition>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartPositionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<NymaTypes.NSwitchInfo> EndNSwitchInfo(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NymaTypes.NSwitchInfo>(o);
  }
  public NSwitchInfoT UnPack() {
    var _o = new NSwitchInfoT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(NSwitchInfoT _o) {
    _o.DefaultPosition = this.DefaultPosition;
    _o.Positions = new List<NymaTypes.NSwitchPositionT>();
    for (var _j = 0; _j < this.PositionsLength; ++_j) {_o.Positions.Add(this.Positions(_j).HasValue ? this.Positions(_j).Value.UnPack() : null);}
  }
  public static Offset<NymaTypes.NSwitchInfo> Pack(FlatBufferBuilder builder, NSwitchInfoT _o) {
    if (_o == null) return default(Offset<NymaTypes.NSwitchInfo>);
    var _Positions = default(VectorOffset);
    if (_o.Positions != null) {
      var __Positions = new Offset<NymaTypes.NSwitchPosition>[_o.Positions.Count];
      for (var _j = 0; _j < __Positions.Length; ++_j) { __Positions[_j] = NymaTypes.NSwitchPosition.Pack(builder, _o.Positions[_j]); }
      _Positions = CreatePositionsVector(builder, __Positions);
    }
    return CreateNSwitchInfo(
      builder,
      _o.DefaultPosition,
      _Positions);
  }
}

public class NSwitchInfoT
{
  public uint DefaultPosition { get; set; }
  public List<NymaTypes.NSwitchPositionT> Positions { get; set; }

  public NSwitchInfoT() {
    this.DefaultPosition = 0;
    this.Positions = null;
  }
}


}
