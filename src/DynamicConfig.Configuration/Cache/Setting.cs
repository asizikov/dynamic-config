namespace DynamicConfig.Configuration.Cache {
  public sealed class Setting<TType> {
    public TType Value { get; }

    public Setting(TType value) {
      Value = value;
    }
  }
}