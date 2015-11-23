using System.Collections;

public class XMLNodeList: ArrayList 
{
	public XMLNode First()
	{
		return (XMLNode)this[0];
	}
	
	public XMLNode Pop()
	{
		XMLNode item = (XMLNode)this[this.Count - 1];
		this.Remove(item);
		
		return item;
	}
	
	public int Push(XMLNode item)
	{
		Add(item);
		
		return this.Count;
	}
}