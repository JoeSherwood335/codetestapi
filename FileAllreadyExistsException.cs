namespace codetestapi {

  [System.Serializable]
  public class FileAllreadyExistsException : System.Exception
  {
      public FileAllreadyExistsException() { }
      public FileAllreadyExistsException(string message) : base(message) { }
      public FileAllreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
      protected FileAllreadyExistsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }
}