using System;
// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
using System.Collections.Generic;
// Issue 10 - End
using System.Windows.Forms;

using TheBox.Common;

namespace TheBox.Data
{
	/// <summary>
	/// Provides access to generic scripted items
	/// </summary>
	public class ScriptList
	{
		// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
		private List<object> m_Items;
		// Issue 10 - End

		/// <summary>
		/// Gets or sets the list of generic nodes composing this object
		/// </summary>
		// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
		public List<object> List
		// Issue 10 - End
		{
			get
			{
				return m_Items;
			}
			set
			{
				m_Items = value;
			}
		}

		/// <summary>
		/// Creates a new ScriptList object
		/// </summary>
		public ScriptList()
		{
			// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
			m_Items = new List<object>();
			// Issue 10 - End
		}

		/// <summary>
		/// Creates a new ScriptList object and initializes the contents
		/// </summary>
		/// <param name="items">The List<> to use as content</param>
		// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
		public ScriptList( List<object> items)
		// Issue 10 - End
		{
			m_Items = items;
		}

		/// <summary>
		/// Updates the contents of the listing from a TreeNodeCollection object
		/// </summary>
		/// <param name="nodes">The collection of nodes holding the information to store</param>
		/// <param name="exclusions">A list of TreeNode items that should be excluded</param>
		public void Update( TreeNodeCollection nodes, params TreeNode[] exclusions )
		{
			m_Items.Clear();

			foreach ( TreeNode node in nodes )
			{
				bool exclude = false;

				foreach ( TreeNode cmp in exclusions )
				{
					if ( node == cmp )
					{
						exclude = true;
						break;
					}
				}

				if ( exclude )
				{
					continue;
				}

				m_Items.Add( DoNode( node ) );
			}

			OnSaving( new EventArgs() );
		}

		/// <summary>
		/// Process and converts a TreeNode
		/// </summary>
		/// <param name="node">The TreeNode to convert</param>
		/// <returns>A GenericNode object corresponding to the TreeNode</returns>
		private GenericNode DoNode( TreeNode node )
		{
			GenericNode gNode = new GenericNode( node.Text );

			// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
			foreach ( object o in ( node.Tag as List<object> ) )
			// Issue 10 - End
				gNode.Elements.Add( o );

			foreach ( TreeNode subNode in node.Nodes )
				gNode.Elements.Add( DoNode( subNode ) );

			return gNode;
		}

		/// <summary>
		/// Converts a GenericNode to a TreeNode
		/// </summary>
		/// <param name="from">The GenericNode to examine</param>
		/// <returns>A TreeNode object corresponding to the GenericNode supplied</returns>
		private TreeNode GetNode( GenericNode from )
		{
			TreeNode node = new TreeNode( from.Name );
			// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
			node.Tag = new List<object>();
			// Issue 10 - End

			foreach ( object o in from.Elements )
			{
				if ( o is GenericNode )
					node.Nodes.Add( GetNode( o as GenericNode ) );
				else
					// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
					( node.Tag as List<object> ).Add( o );
					// Issue 10 - End
			}

			return node;
		}

		/// <summary>
		/// Gets the TreeNodes corresponding to the list
		/// </summary>
		/// <returns></returns>
		public TreeNode[] GetNodes()
		{
			TreeNode[] nodes = new TreeNode[ m_Items.Count ];

			for( int i = 0; i < nodes.Length; i++ )
			{
				if ( m_Items[ i ] is GenericNode )
					nodes[ i ] = GetNode( m_Items[ i ] as GenericNode );
			}

			return nodes;
		}

		/// <summary>
		/// Gets the TreeNodes corresponding to the valid items in the list (ignores generic nodes)
		/// </summary>
		/// <param name="items">The list to evaluate</param>
		/// <returns>null if there are no valid entries in the array list, the corresponding tree nodes otherwise</returns>
		// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
		public TreeNode[] GetDataNodes( List<object> items )
		// Issue 10 - End
		{
			int count = 0;

			foreach ( object o in items )
				if ( ! ( o is GenericNode ) )
					count++;

			if ( count == 0 )
				return null;

			TreeNode[] nodes = new TreeNode[ count ];

			count = 0;

			foreach ( object o in items )
			{
				if ( ! ( o is GenericNode ) )
				{
					TreeNode node = new TreeNode();

					if ( o is BoxItem )
					{
						node.Text = ( o as BoxItem ).Name;
					}
					else if ( o is BoxMobile )
					{
						node.Text = ( o as BoxMobile ).Name;
					}

					node.Tag = o;

					nodes[ count++ ] = node;
				}
			}

			ExpandNames( nodes );
			return nodes;
		}

		/// <summary>
		/// Occurs when the object is being updated and requires saving
		/// </summary>
		public event EventHandler Saving;

		protected virtual void OnSaving( EventArgs e )
		{
			if ( Saving != null )
			{
				Saving( this, e );
			}
		}

		/// <summary>
		/// Adds a space before each uppercase letter
		/// </summary>
		/// <param name="nodes"></param>
		private void ExpandNames( TreeNode[] nodes )
		{
			foreach ( TreeNode node in nodes )
			{
				string text = node.Text;
				int index = 1;

				while ( index < text.Length )
				{
					if ( char.IsUpper( text, index ) )
					{
						if ( index < text.Length - 1 )
						{
							if ( char.IsLower( text, index + 1 ) )
							{
								text = text.Insert( index++, " " );
							}
						}
						else
						{
							// Last char, insert space only if after lowercase
							if ( char.IsLower( text, index - 1 ) )
							{
								text = text.Insert( index++, " " );
							}
						}
					}

					index++;
				}

				node.Text = text;
			}
		}
	}
}
