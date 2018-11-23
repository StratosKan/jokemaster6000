public interface ISerializable
{
    void Serialize(string filePath);

    void Deserialize(string filePath);
}
