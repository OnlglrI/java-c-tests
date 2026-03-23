package com.example.shop.controller;

import com.example.shop.dto.OrderDto;
import com.example.shop.dto.UserDto;
import com.example.shop.model.Order;
import com.example.shop.model.OrderStatus;
import com.example.shop.model.User;
import com.example.shop.repository.OrderRepository;
import com.example.shop.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/admin")
@PreAuthorize("hasRole('ADMIN')")
@RequiredArgsConstructor
public class AdminController {

    private final UserRepository userRepository;
    private final OrderRepository orderRepository;

    @GetMapping("/users")
    public ResponseEntity<List<UserDto>> getUsers(@RequestParam(required = false) String search) {
        List<User> users = userRepository.findAll();
        if (search != null && !search.isBlank()) {
            String lower = search.toLowerCase();
            users = users.stream()
                    .filter(u -> u.getUsername().toLowerCase().contains(lower)
                            || u.getEmail().toLowerCase().contains(lower))
                    .collect(Collectors.toList());
        }
        List<UserDto> result = users.stream().map(this::toUserDto).collect(Collectors.toList());
        return ResponseEntity.ok(result);
    }

    @PatchMapping("/users/{id}/activate")
    public ResponseEntity<UserDto> toggleUserActive(@PathVariable Long id) {
        User user = userRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found"));
        user.setActive(!user.isActive());
        userRepository.save(user);
        return ResponseEntity.ok(toUserDto(user));
    }

    @GetMapping("/orders")
    public ResponseEntity<List<OrderDto>> getAllOrders() {
        List<Order> orders = orderRepository.findAll();
        List<OrderDto> result = orders.stream().map(this::toOrderDto).collect(Collectors.toList());
        return ResponseEntity.ok(result);
    }

    @PatchMapping("/orders/{id}/status")
    public ResponseEntity<OrderDto> updateOrderStatus(@PathVariable Long id,
                                                       @RequestBody Map<String, String> body) {
        Order order = orderRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Order not found"));
        String statusStr = body.get("status");
        if (statusStr == null) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Status is required");
        }
        try {
            OrderStatus newStatus = OrderStatus.valueOf(statusStr.toUpperCase());
            order.setStatus(newStatus);
            orderRepository.save(order);
        } catch (IllegalArgumentException e) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Invalid status: " + statusStr);
        }
        return ResponseEntity.ok(toOrderDto(order));
    }

    private UserDto toUserDto(User user) {
        UserDto dto = new UserDto();
        dto.setId(user.getId());
        dto.setUsername(user.getUsername());
        dto.setEmail(user.getEmail());
        dto.setRole(user.getRole().name());
        dto.setActive(user.isActive());
        return dto;
    }

    private OrderDto toOrderDto(Order order) {
        OrderDto dto = new OrderDto();
        dto.setId(order.getId());
        dto.setUserId(order.getUser().getId());
        dto.setProductId(order.getProduct().getId());
        dto.setProductName(order.getProduct().getName());
        dto.setQuantity(order.getQuantity());
        dto.setTotalPrice(order.getTotalPrice());
        dto.setStatus(order.getStatus().name());
        return dto;
    }
}
