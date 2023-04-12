namespace Nekai.Common.Interfaces;

public interface IJsonSerializable<TSelf>
{
	abstract Result TrySerialize();
}