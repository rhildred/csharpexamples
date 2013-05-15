using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;
using System;

/// <summary>
/// Strings are a collection of strings that the user can have control of for display on a web page
/// </summary>
public class Strings
{
    MySqlConnection conn = null;
    /// <summary>
    /// make a new collection by connecting to a database with strings in it
    /// </summary>
    /// <param name="sConn">the connection string</param>
	public Strings(String sConn)
	{
        this.conn = new MySqlConnection(sConn);
	}

    public String get(String sKey)
    {
        if (this.conn.State != ConnectionState.Open)
        {
            this.conn.Open();
        }
        MySqlCommand cmd = new MySqlCommand("SELECT userString FROM strings WHERE sKey = @sKey", this.conn);
        cmd.Parameters.AddWithValue("sKey", sKey);
        String rc = (String)cmd.ExecuteScalar();
        return rc;

    }

    /// <summary>
    /// update or insert a key and its value
    /// </summary>
    /// <param name="sKey">the key to be changed</param>
    /// <param name="sValue">the value to set it to</param>
    public void put(String sKey, String sValue)
    {
        if (this.conn.State != ConnectionState.Open)
        {
            this.conn.Open();
        }
        MySqlCommand cmd = new MySqlCommand("UPDATE strings SET userString = @userString WHERE sKey = @sKey", this.conn);
        cmd.Parameters.AddWithValue("sKey", sKey);
        cmd.Parameters.AddWithValue("userString", sValue);
        int nRows = cmd.ExecuteNonQuery();
        if (nRows == 0)
        {
            cmd.CommandText = "INSERT INTO strings(sKey, userString) VALUES(@sKey, @userString)";
            nRows = cmd.ExecuteNonQuery();
        }
        if (nRows != 1)
        {
            throw new Exception(nRows + " changed expected 1. SQL was " + cmd.CommandText);
        }
    }

    /// <summary>
    /// release the database connection
    /// </summary>
    public void Close()
    {
        if (this.conn.State == ConnectionState.Open)
        {
            this.conn.Close();
        }
    }

}