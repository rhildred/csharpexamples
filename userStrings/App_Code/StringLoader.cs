using System.Data;
using MySql.Data.MySqlClient;
using MySql.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StringLoader can retrieve multiple database strings for the same page
/// </summary>
public class StringLoader
{
    MySqlConnection conn = null;
    /// <summary>
    /// makes a string loader that can be used multiple times on the same page
    /// </summary>
    /// <param name="sConn">the connection string</param>
	public StringLoader(String sConn)
	{
        this.conn = new MySqlConnection(sConn);
	}

    /// <summary>
    /// gets a specific string
    /// </summary>
    /// <param name="sKey">the key of the string to retrieve</param>
    /// <returns>the string that belongs with that key</returns>
    public String get(String sKey)
    {
        if (this.conn.State != ConnectionState.Open)
        {
            this.conn.Open();
        }
        MySqlCommand cmd = new MySqlCommand("SELECT outputText FROM strings WHERE skey = @skey", this.conn);
        cmd.Parameters.AddWithValue("skey", sKey);
        return (String)cmd.ExecuteScalar();
    }

    /// <summary>
    /// insert or update a key
    /// </summary>
    /// <param name="sKey">key to be updated</param>
    /// <param name="sValue">value to set it to</param>
    public void put(String sKey, String sValue)
    {
        if (this.conn.State != ConnectionState.Open)
        {
            this.conn.Open();
        }
        // first try an update as that is the most likely case
        MySqlCommand cmd = new MySqlCommand("UPDATE strings SET outputText = @outputText WHERE skey = @skey", this.conn);
        cmd.Parameters.AddWithValue("skey", sKey);
        cmd.Parameters.AddWithValue("outputText", sValue);
        int nUpdated = cmd.ExecuteNonQuery();
        if (0 == nUpdated)
        {
            // if that doesn't work try an insert
            cmd.CommandText = "INSERT INTO strings(skey, outputText) VALUES(@skey, @outputText)";
            nUpdated = cmd.ExecuteNonQuery();
        }
        if(nUpdated != 1)
        {
            // if we didn't update exactly one row
            throw new Exception(nUpdated + " updated or inserted expected 1. SQL was " + cmd.CommandText);
        }
    }


    /// <summary>
    /// need to close database connection
    /// </summary>
    public void Close()
    {
        conn.Close();
    }
}