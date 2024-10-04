import React, { useState, useEffect } from 'react';
import { Grid, TextField, Select, MenuItem, Table, TableHead, TableRow, TableCell, TableBody, Button, IconButton } from '@mui/material';
import axios from 'axios';

const MentorPendingProjects = () => {
  const [projects, setProjects] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [filter, setFilter] = useState('all');

  useEffect(() => {
    const fetchProjects = async () => {
      try {
        const response = await axios.get('https://localhost:7289/api/projects/pending', {
            withCredentials: true, // Thêm tùy chọn này
        });
        setProjects(response.data.data);
      } catch (error) {
        console.error('Error fetching projects:', error);
      }
    };

    fetchProjects();
  }, []);

  const handleSearchChange = (event) => {
    setSearchQuery(event.target.value);
  };

  const handleFilterChange = (event) => {
    setFilter(event.target.value);
  };

  const handleAccept = (projectId) => {
    // Thực hiện API chấp nhận
    console.log('Accept project with id:', projectId);
  };

  const handleReject = (projectId) => {
    // Thực hiện API từ chối
    console.log('Reject project with id:', projectId);
  };

  const filteredProjects = projects.filter((project) => {
    return (
      (filter === 'all' || project.status === filter) &&
      (project.groupName.toLowerCase().includes(searchQuery.toLowerCase()) ||
        project.topicName.toLowerCase().includes(searchQuery.toLowerCase()))
    );
  });

  return (
    <div>
      <Grid container spacing={2} alignItems="center">
        <Grid item xs={12} sm={6}>
          <TextField
            label="Search"
            variant="outlined"
            fullWidth
            value={searchQuery}
            onChange={handleSearchChange}
          />
        </Grid>
        <Grid item xs={12} sm={6}>
          <Select
            label="Filter"
            value={filter}
            onChange={handleFilterChange}
            fullWidth
          >
            <MenuItem value="all">All</MenuItem>
            <MenuItem value="pending">Chờ Phê Duyệt</MenuItem>
            <MenuItem value="approved">Đã Chấp Nhận</MenuItem>
            <MenuItem value="rejected">Đã Từ Chối</MenuItem>
          </Select>
        </Grid>
      </Grid>

      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Group Name</TableCell>
            <TableCell>Topic Name</TableCell>
            <TableCell>Description</TableCell>
            <TableCell align="center">Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {filteredProjects.map((project) => (
            <TableRow key={project.id}>
              <TableCell>{project.groupName}</TableCell>
              <TableCell>{project.topic}</TableCell>
              <TableCell>{project.description}</TableCell>
              <TableCell align="center">
                <Button
                  variant="contained"
                  color="primary"
                  onClick={() => handleAccept(project.id)}
                >
                  Accept
                </Button>
                <Button
                  variant="contained"
                  color="secondary"
                  onClick={() => handleReject(project.id)}
                  style={{ marginLeft: '10px' }}
                >
                  Reject
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};

export default MentorPendingProjects;